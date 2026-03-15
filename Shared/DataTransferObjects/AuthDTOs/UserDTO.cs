using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DataTransferObjects.AuthDTOs
{
    public class UserDTO
    {
        public string Email { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Token { get; set; } = null!;
        public string Role { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
        public bool MustChangePassword { get; set; }
    }
}
