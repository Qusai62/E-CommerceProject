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
    public class VariantAttributeConfiguration : IEntityTypeConfiguration<VariantAttribute>
    {
        public void Configure(EntityTypeBuilder<VariantAttribute> builder)
        {
            builder.ToTable("VariantAttribute");

            // Primary Key
            builder.HasKey(x => x.VariantAttributeId)
                .HasName("PK_VariantAttribute");

            builder.Property(x => x.VariantAttributeId)
                .ValueGeneratedOnAdd();

            // ProductVariant Relationship
            builder.Property(x => x.ProductVariantId)
                .IsRequired();

            builder.HasOne(x => x.ProductVariant)
                .WithMany(x => x.Attributes)
                .HasForeignKey(x => x.ProductVariantId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName(
                    "FK_VariantAttribute_ProductVariant_ProductVariantId");

            // Attribute Information
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Value)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.SortOrder)
                .IsRequired();

            // One attribute name per Variant
            builder.HasIndex(x => new
            {
                x.ProductVariantId,
                x.Name
            })
            .IsUnique()
            .HasDatabaseName(
                "UQ_VariantAttribute_ProductVariantId_Name");

            // Useful when loading attributes for a Variant
            builder.HasIndex(x => new
            {
                x.ProductVariantId,
                x.SortOrder
            })
            .HasDatabaseName(
                "IX_VariantAttribute_ProductVariantId_SortOrder");

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
