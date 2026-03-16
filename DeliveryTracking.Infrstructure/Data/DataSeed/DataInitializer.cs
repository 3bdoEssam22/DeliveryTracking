using Delivery_Tracking.Core.Contracts;
using Delivery_Tracking.Core.Entities.SecurityModule;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace DeliveryTracking.Infrstructure.Data.DataSeed
{
    public class DataInitializer(UserManager<DeliveryTrackingUser> _userManager, RoleManager<IdentityRole> _roleManager, IConfiguration _configuration)
        : IDataInitializer
    {
        public async Task InitializeAdminAndRolesAsync()
        {
            // 1. Create roles if they don't exist
            string[] roles = ["Admin", "Driver", "Customer"];
            foreach (var role in roles)
                if (!await _roleManager.RoleExistsAsync(role))
                    await _roleManager.CreateAsync(new IdentityRole(role));

            // 2. Create default Admin if no admin exists
            var adminEmail = _configuration["AdminSettings:Email"] ?? "admin@deliverytracking.com";
            var adminPassword = _configuration["AdminSettings:Password"] ?? "P@ssw0rd123";
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
                await _userManager.CreateAsync(admin, adminPassword);
                await _userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }
}
