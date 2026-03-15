using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shared.DataTransferObjects.AuthDTOs
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "Email address is required!")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Enter a valid Email")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is required!")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Password is required!")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = null!;


        [Required(ErrorMessage = "Phone Number address is required!")]
        [DataType(DataType.PhoneNumber)]
        [Phone(ErrorMessage = "Enter a valid phone number")]
        public string PhoneNumber { get; set; } = null!;

        [Required(ErrorMessage = "Full name is required!")]
        public string FullName { get; set; } = null!;
    }
}
