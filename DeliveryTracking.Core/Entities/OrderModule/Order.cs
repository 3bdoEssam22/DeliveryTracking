using DeliveryTracking.Core.Entities.SecurityModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeliveryTracking.Core.Entities.OrderModule
{
    public class Order :BaseEntity<Guid>
    {
        public string CustomerId { get; set; } = null!;
        public string? DriverId { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public string DeliveryAddress { get; set; } = null!;
        public string? Notes { get; set; }
        public string? CancellationReason { get; set; }
        public DateTime? DeliveredAt { get; set; }

        public DeliveryTrackingUser Customer { get; set; } = null!;
        public DeliveryTrackingUser? Driver { get; set; }
        public List<OrderItem> Items { get; set; } = [];
    }
}
