using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects.OrderDTOs
{
    public class AssignDriverDTO
    {
        [Required(ErrorMessage = "Driver ID is required")]
        public string DriverId { get; set; } = null!;
    }
}
