using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DataTransferObjects.OrderDTOs
{
    public class OrderItemResponseDTO
    {
        public string ProductName { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
