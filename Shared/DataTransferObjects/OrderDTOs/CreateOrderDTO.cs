using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shared.DataTransferObjects.OrderDTOs
{
    public class CreateOrderDTO
    {
        [Required(ErrorMessage = "Delivery address is required")]
        [MaxLength(300)]
        public string DeliveryAddress { get; set; } = null!;

        public string? Notes { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "Order must have at least one item")]
        public List<CreateOrderItemDTO> Items { get; set; } = [];
    }
}
