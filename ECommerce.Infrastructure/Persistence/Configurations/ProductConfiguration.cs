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
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Product");

            // Primary Key
            builder.HasKey(x => x.ProductId)
                .HasName("PK_Product");

            builder.Property(x => x.ProductId)
                .ValueGeneratedOnAdd();

            // Product Information
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Slug)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Description)
                .HasMaxLength(2000);

            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(false);

            // Brand Relationship
            builder.HasOne(x => x.Brand)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.BrandId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Product_Brand_BrandId");

            // Category Relationship
            builder.HasOne(x => x.Category)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Product_Category_CategoryId");

            // Unique Slug
            builder.HasIndex(x => x.Slug)
                .IsUnique()
                .HasDatabaseName("UQ_Product_Slug");

            // Query Indexes
            builder.HasIndex(x => new
            {
                x.CategoryId,
                x.IsActive
            })
            .HasDatabaseName("IX_Product_CategoryId_IsActive");

            builder.HasIndex(x => new
            {
                x.BrandId,
                x.IsActive
            })
            .HasDatabaseName("IX_Product_BrandId_IsActive");

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
