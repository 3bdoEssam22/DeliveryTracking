namespace DeliveryTracking.Core.Entities.OrderModule
{
    public class OrderItem
    {
        public Guid OrderId { get; set; }
        public string ProductName { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }

        public Order Order { get; set; } = null!;
    }
}