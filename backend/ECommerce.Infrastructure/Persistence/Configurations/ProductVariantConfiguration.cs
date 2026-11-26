using ECommerce.Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Persistence.Configurations
{
    public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
    {
        public void Configure(EntityTypeBuilder<ProductVariant> builder)
        {
            builder.ToTable(
                "ProductVariant",
                table =>
                {
                    table.HasCheckConstraint(
                        "CK_ProductVariant_Price_Positive",
                        "[Price] > 0");
                });

            // Primary Key
            builder.HasKey(x => x.ProductVariantId)
                .HasName("PK_ProductVariant");

            builder.Property(x => x.ProductVariantId)
                .ValueGeneratedOnAdd();

            // Product Relationship
            builder.Property(x => x.ProductId)
                .IsRequired();

            builder.HasOne(x => x.Product)
                .WithMany(x => x.Variants)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName(
                    "FK_ProductVariant_Product_ProductId");

            // SKU
            builder.Property(x => x.Sku)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(x => x.Sku)
                .IsUnique()
                .HasDatabaseName("UQ_ProductVariant_Sku");

            // Price
            builder.Property(x => x.Price)
                .IsRequired()
                .HasPrecision(18, 3);

            // Active State
            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(false);

            // Common Product Variant Query
            builder.HasIndex(x => new
            {
                x.ProductId,
                x.IsActive
            })
            .HasDatabaseName(
                "IX_ProductVariant_ProductId_IsActive");

            // Optimistic Concurrency
            builder.Property<byte[]>("RowVersion")
                .IsRowVersion()
                .IsConcurrencyToken();

            // Audit Fields
            builder.Property(x => x.CreatedAtUtc)
                .IsRequired()
                .HasColumnType("datetime2(3)");

            builder.Property(x => x.UpdatedAtUtc)
                .IsRequired()
                .HasColumnType("datetime2(3)");
        }
    }
}
