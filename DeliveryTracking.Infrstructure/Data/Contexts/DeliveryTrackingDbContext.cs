using Delivery_Tracking.Core.Entities.SecurityModule;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DeliveryTracking.Infrstructure.Data.Contexts
{
    public class DeliveryTrackingDbContext(DbContextOptions<DeliveryTrackingDbContext> options) :
        IdentityDbContext<DeliveryTrackingUser, IdentityRole, string>(options)
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityRole>().ToTable("Roles");

            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
