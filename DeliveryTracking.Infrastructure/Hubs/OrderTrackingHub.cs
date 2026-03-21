
using DeliveryTracking.Core.Contracts;
using DeliveryTracking.Core.Entities.OrderModule;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DeliveryTracking.Infrastructure.Hubs
{
    [Authorize]
    public class OrderTrackingHub(IUnitOfWork _unitOfWork) : Hub
    {
        public async Task JoinOrderGroup(string orderId)
        {
            var userId = Context.UserIdentifier;
            if (string.IsNullOrEmpty(userId))
            {
                await Clients.Caller.SendAsync("Error", "Unauthorized.");
                return;
            }

            if (!Guid.TryParse(orderId, out var orderGuid))
            {
                await Clients.Caller.SendAsync("Error", "Invalid order ID.");
                return;
            }

            var order = await _unitOfWork.GetRepository<Order, Guid>().GetByIdAsync(orderGuid);

            if (order is null || order.CustomerId != userId)
            {
                await Clients.Caller.SendAsync("Error", "Order not found or access denied.");
                return;
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, orderId);
            await Clients.Caller.SendAsync("JoinedGroup", orderId);
        }

        public async Task LeaveOrderGroup(string orderId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, orderId);
        }
    }
}