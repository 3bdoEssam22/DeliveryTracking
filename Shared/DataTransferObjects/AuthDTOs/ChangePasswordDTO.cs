using System;
using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects.AuthDTOs
{
    public class ChangePasswordDTO
    {
        [Required(ErrorMessage = "Current password is required!")]
        public string CurrentPassword { get; set; } = null!;

        [Required(ErrorMessage = "New password is required!")]
        public string NewPassword { get; set; } = null!;
    }
}
