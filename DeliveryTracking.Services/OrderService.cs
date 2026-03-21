using AutoMapper;
using DeliveryTracking.Core.Contracts;
using DeliveryTracking.Core.Entities.OrderModule;
using DeliveryTracking.Core.Entities.SecurityModule;
using DeliveryTracking.Services.Abstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Shared.DataTransferObjects.OrderDTOs;
using Shared.Messages;
using Shared.Responses;
using System.Linq.Expressions;
namespace DeliveryTracking.Services
{
    public class OrderService(
    IUnitOfWork _unitOfWork,
    IMapper _mapper,
    UserManager<DeliveryTrackingUser> _userManager,
    IOrderNotificationService _notifications,
    IEmailService _emailService) : IOrderService
    {
        // ─── Create
        public async Task<GenericResponse<OrderResponseDTO>> CreateOrderAsync(string customerId, CreateOrderDTO dto)
        {
            var response = new GenericResponse<OrderResponseDTO>();

            if (dto.Items == null || dto.Items.Count == 0)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = "Order must have at least one item.";
                return response;
            }

            if (dto.Items.Any(i => i.Quantity < 1 || i.UnitPrice <= 0))
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = "All items must have quantity >= 1 and unit price > 0.";
                return response;
            }

            var items = dto.Items.Select(i => new OrderItem
            {
                Id = Guid.NewGuid(),
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                TotalPrice = i.Quantity * i.UnitPrice,
            }).ToList();

            var order = new Order
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                Status = OrderStatus.Pending,
                DeliveryAddress = dto.DeliveryAddress,
                Notes = dto.Notes,
                TotalAmount = items.Sum(i => i.TotalPrice),
                Items = items
            };

            await _unitOfWork.GetRepository<Order, Guid>().AddAsync(order);
            await _unitOfWork.SaveChangesAsync();

            var savedOrder = await _unitOfWork.GetRepository<Order, Guid>()
                                    .GetByIdAsync(order.Id, [o => o.Customer, o => o.Driver!, o => o.Items]);

            if (savedOrder is null)
            {
                response.StatusCode = StatusCodes.Status500InternalServerError;
                response.Message = "Order was created, but failed to load the created order.";
                return response;
            }

            // Notify all admins
            var admins = await _userManager.GetUsersInRoleAsync("Admin");
            foreach (var admin in admins)
                await _notifications.PushNotificationAsync(admin.Id, "New order placed.", "OrderPlaced");

            response.StatusCode = StatusCodes.Status201Created;
            response.Message = "Order created successfully.";
            response.Data = _mapper.Map<OrderResponseDTO>(savedOrder);
            return response;
        }

        // ─── Get by ID
        public async Task<GenericResponse<OrderResponseDTO>> GetOrderByIdAsync(string requesterId, string requesterRole, Guid orderId)
        {
            var response = new GenericResponse<OrderResponseDTO>();

            var order = await _unitOfWork.GetRepository<Order, Guid>()
                .GetByIdAsync(orderId, [o => o.Customer, o => o.Driver!, o => o.Items]);
            if (order is null)
            {
                response.StatusCode = StatusCodes.Status404NotFound;
                response.Message = "Order not found.";
                return response;
            }

            if (requesterRole == "Customer" && order.CustomerId != requesterId)
            {
                response.StatusCode = StatusCodes.Status403Forbidden;
                response.Message = "Access denied.";
                return response;
            }

            if (requesterRole == "Driver" && order.DriverId != requesterId)
            {
                response.StatusCode = StatusCodes.Status403Forbidden;
                response.Message = "Access denied.";
                return response;
            }

            response.StatusCode = StatusCodes.Status200OK;
            response.Message = "Order retrieved successfully.";
            response.Data = _mapper.Map<OrderResponseDTO>(order);
            return response;
        }

        // ─── Get My Orders
        public async Task<GenericResponse<List<OrderResponseDTO>>> GetMyOrdersAsync(string userId, string role)
        {
            var response = new GenericResponse<List<OrderResponseDTO>>();

            Expression<Func<Order, bool>> criteria = role == "Driver"
                ? o => o.DriverId == userId
                : o => o.CustomerId == userId;

            var orders = await _unitOfWork.GetRepository<Order, Guid>()
                .GetAllWhereAsync(criteria, [o => o.Customer, o => o.Driver!, o => o.Items]);

            var ordered = orders.OrderByDescending(o => o.CreatedAt).ToList();

            response.StatusCode = StatusCodes.Status200OK;
            response.Message = "Orders retrieved successfully.";
            response.Data = _mapper.Map<List<OrderResponseDTO>>(ordered);
            return response;
        }

        // ─── Get All (Admin)
        public async Task<GenericResponse<List<OrderResponseDTO>>> GetAllOrdersAsync()
        {
            var response = new GenericResponse<List<OrderResponseDTO>>();

            var orders = (await _unitOfWork.GetRepository<Order, Guid>()
                            .GetAllAsync([o => o.Customer, o => o.Driver!, o => o.Items]))
                            .OrderByDescending(o => o.CreatedAt)
                            .ToList();


            response.StatusCode = StatusCodes.Status200OK;
            response.Message = "Orders retrieved successfully.";
            response.Data = _mapper.Map<List<OrderResponseDTO>>(orders);
            return response;
        }

        // ─── Assign Driver
        public async Task<GenericResponse<bool>> AssignDriverAsync(Guid orderId, AssignDriverDTO dto)
        {
            var response = new GenericResponse<bool>();

            var order = await _unitOfWork.GetRepository<Order, Guid>().GetByIdAsync(orderId);
            if (order is null)
            {
                response.StatusCode = StatusCodes.Status404NotFound;
                response.Message = "Order not found.";
                return response;
            }

            if (order.Status != OrderStatus.Pending)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = "Only Pending orders can be assigned a driver.";
                return response;
            }

            var driver = await _userManager.FindByIdAsync(dto.DriverId);
            if (driver is null)
            {
                response.StatusCode = StatusCodes.Status404NotFound;
                response.Message = "Driver not found.";
                return response;
            }

            if (!await _userManager.IsInRoleAsync(driver, "Driver"))
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = "The specified user is not a Driver.";
                return response;
            }

            var activeDriverOrders = await _unitOfWork.GetRepository<Order, Guid>()
                .GetAllWhereAsync(o =>
                    o.DriverId == dto.DriverId &&
                    o.Status != OrderStatus.Delivered &&
                    o.Status != OrderStatus.Cancelled);

            var isBusy = activeDriverOrders.Any();

            if (isBusy)
            {
                response.StatusCode = StatusCodes.Status409Conflict;
                response.Message = "Driver is already assigned to an active order.";
                return response;
            }

            order.DriverId = dto.DriverId;
            order.Status = OrderStatus.Assigned;
            order.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.GetRepository<Order, Guid>().Update(order);
            await _unitOfWork.SaveChangesAsync();

            await _notifications.BroadcastStatusUpdateAsync(orderId, "Assigned");
            await _notifications.PushNotificationAsync(order.CustomerId, "Your order has been assigned to a driver.", "DriverAssigned");
            await _notifications.PushNotificationAsync(dto.DriverId, "You have been assigned a new order.", "DriverAssigned");

            response.StatusCode = StatusCodes.Status200OK;
            response.Message = "Driver assigned successfully.";
            response.Data = true;
            return response;
        }

        // ─── Update Status
        public async Task<GenericResponse<bool>> UpdateStatusAsync(string driverId, Guid orderId, UpdateOrderStatusDTO dto)
        {
            var response = new GenericResponse<bool>();

            var order = await _unitOfWork.GetRepository<Order, Guid>().GetByIdAsync(orderId);
            if (order is null)
            {
                response.StatusCode = StatusCodes.Status404NotFound;
                response.Message = "Order not found.";
                return response;
            }

            if (order.DriverId != driverId)
            {
                response.StatusCode = StatusCodes.Status403Forbidden;
                response.Message = "You are not assigned to this order.";
                return response;
            }

            var newStatus = (OrderStatus)dto.Status;
            if (!IsValidTransition(order.Status, newStatus))
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = $"Cannot transition from {order.Status} to {newStatus} directly.";
                return response;
            }

            order.Status = newStatus;
            order.UpdatedAt = DateTime.UtcNow;

            if (newStatus == OrderStatus.Delivered)
                order.DeliveredAt = DateTime.UtcNow;

            _unitOfWork.GetRepository<Order, Guid>().Update(order);
            await _unitOfWork.SaveChangesAsync();

            await _notifications.BroadcastStatusUpdateAsync(orderId, newStatus.ToString());
            await _notifications.PushNotificationAsync(order.CustomerId, $"Your order status updated to {newStatus}.", "OrderUpdate");

            if (newStatus == OrderStatus.Delivered)
            {
                var customer = await _userManager.FindByIdAsync(order.CustomerId);
                if (customer?.Email is not null)
                    await _emailService.SendEmailAsync(new Email
                    {
                        To = customer.Email,
                        Subject = "Your order has been delivered!",
                        Body = $"Hi {customer.FullName}, your order has been delivered. Thank you for using DeliveryTracking!"
                    });
            }

            response.StatusCode = StatusCodes.Status200OK;
            response.Message = "Order status updated successfully.";
            response.Data = true;
            return response;
        }

        // ─── Cancel
        public async Task<GenericResponse<bool>> CancelOrderAsync(string requesterId, string requesterRole, Guid orderId, CancelOrderDTO dto)
        {
            var response = new GenericResponse<bool>();

            var order = await _unitOfWork.GetRepository<Order, Guid>().GetByIdAsync(orderId);
            if (order is null)
            {
                response.StatusCode = StatusCodes.Status404NotFound;
                response.Message = "Order not found.";
                return response;
            }

            if (requesterRole == "Customer")
            {
                if (order.CustomerId != requesterId)
                {
                    response.StatusCode = StatusCodes.Status403Forbidden;
                    response.Message = "Access denied.";
                    return response;
                }

                if (order.Status != OrderStatus.Pending)
                {
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = "Customers can only cancel Pending orders.";
                    return response;
                }
            }
            else if (requesterRole == "Admin")
            {
                if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Assigned)
                {
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = "Admins can only cancel Pending or Assigned orders.";
                    return response;
                }
            }

            var driverIdBeforeCancel = order.DriverId;

            order.Status = OrderStatus.Cancelled;
            order.CancellationReason = dto.CancellationReason;
            order.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.GetRepository<Order, Guid>().Update(order);
            await _unitOfWork.SaveChangesAsync();

            await _notifications.BroadcastStatusUpdateAsync(orderId, "Cancelled");

            if (driverIdBeforeCancel is not null)
                await _notifications.PushNotificationAsync(driverIdBeforeCancel, "An order assigned to you has been cancelled.", "Cancelled");

            response.StatusCode = StatusCodes.Status200OK;
            response.Message = "Order cancelled successfully.";
            response.Data = true;
            return response;
        }

        // ─── Helpers
        private static bool IsValidTransition(OrderStatus current, OrderStatus next)
        {
            return (current, next) switch
            {
                (OrderStatus.Assigned, OrderStatus.PickedUp) => true,
                (OrderStatus.PickedUp, OrderStatus.OnTheWay) => true,
                (OrderStatus.OnTheWay, OrderStatus.Delivered) => true,
                _ => false
            };
        }
    }
}
