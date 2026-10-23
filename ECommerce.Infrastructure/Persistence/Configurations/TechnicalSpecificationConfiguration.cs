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
    public class TechnicalSpecificationConfiguration : IEntityTypeConfiguration<TechnicalSpecification>
    {
        public void Configure(
        EntityTypeBuilder<TechnicalSpecification> builder)
        {
            builder.ToTable("TechnicalSpecification");

            // Primary Key
            builder.HasKey(x => x.TechnicalSpecificationId)
                .HasName("PK_TechnicalSpecification");

            builder.Property(x => x.TechnicalSpecificationId)
                .ValueGeneratedOnAdd();

            // Product Relationship
            builder.Property(x => x.ProductId)
                .IsRequired();

            builder.HasOne(x => x.Product)
                .WithMany(x => x.TechnicalSpecifications)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName(
                    "FK_TechnicalSpecification_Product_ProductId");

            // Specification Information
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Value)
                .IsRequired()//makes the database column non-nullable. t does not automatically reject whitespace-only strings.
                .HasMaxLength(500);

            builder.Property(x => x.SortOrder)
                .IsRequired();

            // A Product cannot have the same specification name twice
            builder.HasIndex(x => new
            {
                x.ProductId,
                x.Name
            })
            .IsUnique()
            .HasDatabaseName(
                "UQ_TechnicalSpecification_ProductId_Name");

            // Useful for loading specifications by Product
            builder.HasIndex(x => new
            {
                x.ProductId,
                x.SortOrder
            })
            .HasDatabaseName(
                "IX_TechnicalSpecification_ProductId_SortOrder");

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
