using ECommerce.Domain.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Persistence.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable(
                "Order",
                table =>
                {
                    table.HasCheckConstraint(
                        "CK_Order_ShippingCharge_NonNegative",
                        "[ShippingCharge] >= 0");

                    table.HasCheckConstraint(
                        "CK_Order_TotalAmount_Positive",
                        "[TotalAmount] > 0");

                    table.HasCheckConstraint(
                        "CK_Order_FulfillmentStatus_Valid",
                        "[FulfillmentStatus] IN (1, 2, 3, 4)");
                });

            // Primary Key
            builder.HasKey(x => x.OrderId)
                .HasName("PK_Order");

            builder.Property(x => x.OrderId)
                .ValueGeneratedOnAdd();

            // Order Number
            builder.Property(x => x.OrderNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(x => x.OrderNumber)
                .IsUnique()
                .HasDatabaseName("UQ_Order_OrderNumber");

            // Customer Relationship
            builder.Property(x => x.CustomerId)
                .IsRequired();

            builder.HasOne(x => x.Customer)
                .WithMany(x => x.Orders)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName(
                    "FK_Order_Customer_CustomerId");

            // Fulfillment Status
            builder.Property(x => x.FulfillmentStatus)
     .IsRequired()
     .HasConversion<int>();

            // Monetary Values
            builder.Property(x => x.ShippingCharge)
                .IsRequired()
                .HasPrecision(18, 3);

            builder.Property(x => x.TotalAmount)
                .IsRequired()
                .HasPrecision(18, 3);

            // Delivery Address Snapshot
            builder.Property(x => x.DeliveryRecipientName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.DeliveryContactNumber)
                .IsRequired()
                .HasMaxLength(30);

            builder.Property(x => x.DeliveryCity)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.DeliveryNeighborhood)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.DeliveryStreet)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.DeliveryBuildingInfo)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.DeliveryAdditionalInfo)
                .HasMaxLength(500);

            // Timestamps
            builder.Property(x => x.PlacedAtUtc)
                .IsRequired()
                .HasColumnType("datetime2(3)");

            builder.Property(x => x.UpdatedAtUtc)
                .IsRequired()
                .HasColumnType("datetime2(3)");

            // Optimistic Concurrency
            builder.Property<byte[]>("RowVersion")
                .IsRowVersion()
                .IsConcurrencyToken();

            // Common Query Indexes
            builder.HasIndex(x => new
            {
                x.CustomerId,
                x.PlacedAtUtc
            })
            .HasDatabaseName(
                "IX_Order_CustomerId_PlacedAtUtc");

            builder.HasIndex(x => new
            {
                x.FulfillmentStatus,
                x.PlacedAtUtc
            })
            .HasDatabaseName(
                "IX_Order_FulfillmentStatus_PlacedAtUtc");
        }
    }
}
