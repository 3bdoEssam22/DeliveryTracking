using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects.OrderDTOs
{
    public class CancelOrderDTO
    {
        [Required(ErrorMessage = "Cancellation reason is required")]
        [MaxLength(300)]
        public string CancellationReason { get; set; } = null!;
    }
}
