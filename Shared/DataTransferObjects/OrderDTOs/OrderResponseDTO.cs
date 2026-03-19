using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DataTransferObjects.OrderDTOs
{
    public class OrderResponseDTO
    {
        public Guid Id { get; set; }
        public string Status { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public string DeliveryAddress { get; set; } = null!;
        public string? Notes { get; set; }
        public string? CancellationReason { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public string CustomerName { get; set; } = null!;
        public string? DriverName { get; set; }
        public List<OrderItemResponseDTO> Items { get; set; } = [];
    }
}
