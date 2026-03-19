using DeliveryTracking.Core.Entities.OrderModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeliveryTracking.Infrastructure.Data.Configurations
{
    internal class OrderConfiguration:BaseConfiguration<Order,Guid>
    {
        public override void Configure(EntityTypeBuilder<Order> builder)
        {
            base.Configure(builder);

            builder.ToTable("Orders");

            builder.Property(p => p.TotalAmount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(p => p.DeliveryAddress)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(p => p.CancellationReason)
                .HasMaxLength(500);

            builder.Property(p => p.Status)
                .IsRequired()
                .HasConversion<string>();

            // Customer — restrict delete (cannot delete a user who has orders)
            builder.HasOne(o => o.Customer)
                .WithMany()
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Driver — set null on delete (if driver deleted, order stays)
            builder.HasOne(o => o.Driver)
                .WithMany()
                .HasForeignKey(o => o.DriverId)
                .OnDelete(DeleteBehavior.SetNull);

            // Items — cascade delete (items cannot exist without an order)
            builder.HasMany(o => o.Items)
                .WithOne(i => i.Order)
                .HasForeignKey(i => i.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
