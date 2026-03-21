using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects.OrderDTOs
{
    public class CancelOrderDTO
    {
        public string? CancellationReason { get; set; }
    }
}
