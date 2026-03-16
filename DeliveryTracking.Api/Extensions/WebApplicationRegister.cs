using Delivery_Tracking.Core.Contracts;
using DeliveryTracking.Infrstructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace DeliveryTracking.Api.Extensions
{
    public static class WebApplicationRegister
    {
        public static async Task<WebApplication> MigrateDatabaseAsync(this WebApplication app)
        {
            await using var scope = app.Services.CreateAsyncScope();
            var DtDbContext = scope.ServiceProvider.GetRequiredService<DeliveryTrackingDbContext>();
            var pendingMigrations = await DtDbContext.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
                await DtDbContext.Database.MigrateAsync();
            return app;
        }

        public static async Task<WebApplication> SeedingIdentityData(this WebApplication app)
        {
            await using var scope = app.Services.CreateAsyncScope();
            var dataInitializer = scope.ServiceProvider.GetRequiredService<IDataInitializer>();
            await dataInitializer.InitializeAdminAndRolesAsync();
            return app;
        }

    }
}
