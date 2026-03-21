using DeliveryTracking.Services.Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DataTransferObjects.OrderDTOs;
using Shared.Responses;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace DeliveryTracking.Api.Controllers
{
    public class OrdersController(IOrderService _orderService) : BaseApiController
    {
        private string UserId =>
           User.FindFirstValue(JwtRegisteredClaimNames.NameId)
           ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
           ?? string.Empty;

        private string UserRole =>
            User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

        // POST api/orders
        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult<GenericResponse<OrderResponseDTO>>> CreateOrderAsync([FromBody] CreateOrderDTO dto)
        {
            var result = await _orderService.CreateOrderAsync(UserId, dto);
            return HandleResponse(result);
        }

        // GET api/orders/{id}
        [HttpGet("{id:guid}")]
        [Authorize]
        public async Task<ActionResult<GenericResponse<OrderResponseDTO>>> GetOrderByIdAsync(Guid id)
        {
            var result = await _orderService.GetOrderByIdAsync(UserId, UserRole, id);
            return HandleResponse(result);
        }

        // GET api/orders/my
        [HttpGet("my")]
        [Authorize]
        public async Task<ActionResult<GenericResponse<List<OrderResponseDTO>>>> GetMyOrdersAsync()
        {
            var result = await _orderService.GetMyOrdersAsync(UserId, UserRole);
            return HandleResponse(result);
        }

        // GET api/orders
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<GenericResponse<List<OrderResponseDTO>>>> GetAllOrdersAsync()
        {
            var result = await _orderService.GetAllOrdersAsync();
            return HandleResponse(result);
        }

        // PUT api/orders/{id}/assign
        [HttpPut("{id:guid}/assign")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<GenericResponse<bool>>> AssignDriverAsync(Guid id, [FromBody] AssignDriverDTO dto)
        {
            var result = await _orderService.AssignDriverAsync(id, dto);
            return HandleResponse(result);
        }

        // PUT api/orders/{id}/status
        [HttpPut("{id:guid}/status")]
        [Authorize(Roles = "Driver")]
        public async Task<ActionResult<GenericResponse<bool>>> UpdateStatusAsync(Guid id, [FromBody] UpdateOrderStatusDTO dto)
        {
            var result = await _orderService.UpdateStatusAsync(UserId, id, dto);
            return HandleResponse(result);
        }

        // PUT api/orders/{id}/cancel
        [HttpPut("{id:guid}/cancel")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<ActionResult<GenericResponse<bool>>> CancelOrderAsync(Guid id, [FromBody] CancelOrderDTO dto)
        {
            var result = await _orderService.CancelOrderAsync(UserId, UserRole, id, dto);
            return HandleResponse(result);
        }
    }
}
