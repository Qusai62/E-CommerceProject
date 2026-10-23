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
    public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
    {
        public void Configure(EntityTypeBuilder<ProductImage> builder)
        {
            builder.ToTable("ProductImage");

            // Primary Key
            builder.HasKey(x => x.ProductImageId)
                .HasName("PK_ProductImage");

            builder.Property(x => x.ProductImageId)
                .ValueGeneratedOnAdd();

            // Product Relationship
            builder.Property(x => x.ProductId)
                .IsRequired();

            builder.HasOne(x => x.Product)
                .WithMany(x => x.Images)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ProductImage_Product_ProductId");

            // Image Information
            builder.Property(x => x.StorageKey)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.AltText)
                .HasMaxLength(250);

            builder.Property(x => x.SortOrder)
                .IsRequired();

            builder.Property(x => x.IsPrimary)
                .IsRequired()
                .HasDefaultValue(false);

            // Retrieve Product images in display order
            builder.HasIndex(x => new
            {
                x.ProductId,
                x.SortOrder
            })
            .HasDatabaseName("IX_ProductImage_ProductId_SortOrder");

            // One Product can have at most one primary image
            builder.HasIndex(x => x.ProductId)
                .IsUnique()
                .HasFilter("[IsPrimary] = 1")
                .HasDatabaseName("UQ_ProductImage_ProductId_Primary");

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
