using ECommerce.Domain.Shipping;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Persistence.Configurations
{
    public class ShipmentConfiguration : IEntityTypeConfiguration<Shipment>
    {
        public void Configure(EntityTypeBuilder<Shipment> builder)
        {
            builder.ToTable(
                "Shipment",
                table =>
                {
                    table.HasCheckConstraint(
                        "CK_Shipment_DeliveryStatus_Valid",
                        "[DeliveryStatus] IN (1, 2, 3)");
                });

            // Primary Key
            builder.HasKey(x => x.ShipmentId)
                .HasName("PK_Shipment");

            builder.Property(x => x.ShipmentId)
                .ValueGeneratedOnAdd();

            // Order Relationship
            builder.Property(x => x.OrderId)
                .IsRequired();

            builder.HasOne(x => x.Order)
                .WithOne(x => x.Shipment)
                .HasForeignKey<Shipment>(x => x.OrderId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName(
                    "FK_Shipment_Order_OrderId");

            // One Shipment per Order
            builder.HasIndex(x => x.OrderId)
                .IsUnique()
                .HasDatabaseName(
                    "UQ_Shipment_OrderId");

            // Delivery Status
            builder.Property(x => x.DeliveryStatus)
                .IsRequired()
                .HasConversion<int>();

            // Tracking Reference
            builder.Property(x => x.TrackingReference)
                .HasMaxLength(200);

            builder.HasIndex(x => x.TrackingReference)
                .IsUnique()
                .HasFilter("[TrackingReference] IS NOT NULL")
                .HasDatabaseName(
                    "UQ_Shipment_TrackingReference");

            // Shipping lifecycle timestamps
            builder.Property(x => x.ShippedAtUtc)
                .HasColumnType("datetime2(3)");

            builder.Property(x => x.DeliveredAtUtc)
                .HasColumnType("datetime2(3)");

            // Audit timestamps
            builder.Property(x => x.CreatedAtUtc)
                .IsRequired()
                .HasColumnType("datetime2(3)");

            builder.Property(x => x.UpdatedAtUtc)
                .IsRequired()
                .HasColumnType("datetime2(3)");

            // Useful for querying shipment queues
            builder.HasIndex(x => x.DeliveryStatus)
                .HasDatabaseName(
                    "IX_Shipment_DeliveryStatus");
        }
    }
}
