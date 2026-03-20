using DeliveryTracking.Core.Entities.OrderModule;
using DeliveryTracking.Core.Entities.SecurityModule;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DeliveryTracking.Infrastructure.Data.Contexts
{
    public class DeliveryTrackingDbContext(DbContextOptions<DeliveryTrackingDbContext> options) :
        IdentityDbContext<DeliveryTrackingUser, IdentityRole, string>(options)
    {
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityRole>().ToTable("Roles");

            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
