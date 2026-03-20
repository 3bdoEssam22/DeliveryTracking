using DeliveryTracking.Core.Entities.SecurityModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeliveryTracking.Infrastructure.Data.Configurations
{
    public class IdentityConfigurations : IEntityTypeConfiguration<DeliveryTrackingUser>
    {
        public void Configure(EntityTypeBuilder<DeliveryTrackingUser> builder)
        {
            builder.ToTable("Users");
            builder.Property(p => p.FullName).IsRequired().HasMaxLength(100);
            builder.Property(p => p.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        }
    }
}
