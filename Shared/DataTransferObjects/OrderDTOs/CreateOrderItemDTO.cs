using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects.OrderDTOs
{
    public class CreateOrderItemDTO
    {
        [Required(ErrorMessage = "Product name is required")]
        public string ProductName { get; set; } = null!;

        [Required]
        [Range(1, 100, ErrorMessage = "Quantity must be between 1 and 100")]
        public int Quantity { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than 0")]
        public decimal UnitPrice { get; set; }
    }
}
