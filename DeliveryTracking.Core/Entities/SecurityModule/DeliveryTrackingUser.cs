using Microsoft.AspNetCore.Identity;

namespace Delivery_Tracking.Core.Entities.SecurityModule
{
    public class DeliveryTrackingUser : IdentityUser
    {
        public string FullName { get; set; } = null!;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
