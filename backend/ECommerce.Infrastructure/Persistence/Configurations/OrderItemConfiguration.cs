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
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable(
                "OrderItem",
                table =>
                {
                    table.HasCheckConstraint(
                        "CK_OrderItem_Quantity_Positive",
                        "[Quantity] > 0");

                    table.HasCheckConstraint(
                        "CK_OrderItem_UnitPrice_Positive",
                        "[UnitPrice] > 0");
                });

            // Primary Key
            builder.HasKey(x => x.OrderItemId)
                .HasName("PK_OrderItem");

            builder.Property(x => x.OrderItemId)
                .ValueGeneratedOnAdd();

            // Order Relationship
            builder.Property(x => x.OrderId)
                .IsRequired();

            builder.HasOne(x => x.Order)
                .WithMany(x => x.Items)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName(
                    "FK_OrderItem_Order_OrderId");

            // ProductVariant Relationship
            builder.Property(x => x.ProductVariantId)
                .IsRequired();

            builder.HasOne(x => x.ProductVariant)
                .WithMany(x => x.OrderItems)
                .HasForeignKey(x => x.ProductVariantId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName(
                    "FK_OrderItem_ProductVariant_ProductVariantId");

            // Quantity
            builder.Property(x => x.Quantity)
                .IsRequired();

            // Historical Price Snapshot
            builder.Property(x => x.UnitPrice)
                .IsRequired()
                .HasPrecision(18, 3);

            // Historical Product Snapshots
            builder.Property(x => x.ProductNameSnapshot)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.SkuSnapshot)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.VariantAttributesSnapshot)
                .IsRequired()
                .HasMaxLength(2000);

            // Prevent duplicate Variant lines inside the same Order
            builder.HasIndex(x => new
            {
                x.OrderId,
                x.ProductVariantId
            })
            .IsUnique()
            .HasDatabaseName(
                "UQ_OrderItem_OrderId_ProductVariantId");

            // Useful when locating historical orders containing a Variant
            builder.HasIndex(x => x.ProductVariantId)
                .HasDatabaseName(
                    "IX_OrderItem_ProductVariantId");

            // Historical creation timestamp
            builder.Property(x => x.CreatedAtUtc)
                .IsRequired()
                .HasColumnType("datetime2(3)");
        }
    }
}
