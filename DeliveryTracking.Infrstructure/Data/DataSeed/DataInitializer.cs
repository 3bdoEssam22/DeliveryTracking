using Delivery_Tracking.Core.Contracts;
using Delivery_Tracking.Core.Entities.SecurityModule;
using Microsoft.AspNetCore.Identity;

namespace DeliveryTracking.Infrstructure.Data.DataSeed
{
    public class DataInitializer(UserManager<DeliveryTrackingUser> _userManager, RoleManager<IdentityRole> _roleManager)
        : IDataInitializer
    {
        public async Task InializeAdminAndRolesAsync()
        {
            // 1. Create roles if they don't exist
            string[] roles = ["Admin", "Driver", "Customer"];
            foreach (var role in roles)
                if (!await _roleManager.RoleExistsAsync(role))
                    await _roleManager.CreateAsync(new IdentityRole(role));

            // 2. Create default Admin if no admin exists
            var adminEmail = "abdulrahman.e.f22@gmail.com";
            if (await _userManager.FindByEmailAsync(adminEmail) is null)
            {
                var admin = new DeliveryTrackingUser
                {
                    FullName = "System Admin",
                    Email = adminEmail,
                    UserName = adminEmail,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                };
                await _userManager.CreateAsync(admin, "P@ssw0rd");
                await _userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }
}
