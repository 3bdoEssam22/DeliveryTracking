using Shared.DataTransferObjects.OrderDTOs;
using Shared.Responses;

namespace DeliveryTracking.Services.Abstraction
{
    public interface IOrderService
    {
        Task<GenericResponse<OrderResponseDTO>> CreateOrderAsync(string customerId, CreateOrderDTO dto);
        Task<GenericResponse<OrderResponseDTO>> GetOrderByIdAsync(string requesterId, string requesterRole, Guid orderId);
        Task<GenericResponse<List<OrderResponseDTO>>> GetMyOrdersAsync(string userId, string role);
        Task<GenericResponse<List<OrderResponseDTO>>> GetAllOrdersAsync();
        Task<GenericResponse<bool>> AssignDriverAsync(Guid orderId, AssignDriverDTO dto);
        Task<GenericResponse<bool>> UpdateStatusAsync(string driverId, Guid orderId, UpdateOrderStatusDTO dto);
        Task<GenericResponse<bool>> CancelOrderAsync(string requesterId, string requesterRole, Guid orderId, CancelOrderDTO dto);
    }
}
