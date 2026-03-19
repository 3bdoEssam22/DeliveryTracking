using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects.OrderDTOs
{
    public class UpdateOrderStatusDTO
    {
        [Required(ErrorMessage = "Status is required")]
        public OrderStatus Status { get; set; }
    }
}
