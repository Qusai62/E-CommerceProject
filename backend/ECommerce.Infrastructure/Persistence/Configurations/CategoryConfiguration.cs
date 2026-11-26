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
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Category");

            // Primary Key
            builder.HasKey(x => x.CategoryId)
                .HasName("PK_Category");

            builder.Property(x => x.CategoryId)
                .ValueGeneratedOnAdd();

            // Category Information
            builder.Property(x => x.Name)
     .IsRequired()
     .HasMaxLength(150)
     .UseCollation("SQL_Latin1_General_CP1_CI_AS");

            builder.Property(x => x.Slug)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Description)
                .HasMaxLength(1000);

            builder.Property(x => x.SortOrder)
                .IsRequired();

            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(false);

            // Self-Reference: Parent Category -> Child Categories
            builder.HasOne(x => x.ParentCategory)
                .WithMany(x => x.ChildCategories)
                .HasForeignKey(x => x.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Category_Category_ParentCategoryId");

            // Unique Slug
            builder.HasIndex(x => x.Slug)
                .IsUnique()
                .HasDatabaseName("UQ_Category_Slug");

            // Prevent duplicate category names under the same parent
            builder.HasIndex(x => new
            {
                x.ParentCategoryId,
                x.Name
            })
 .IsUnique()
 .HasFilter(null)
 .HasDatabaseName("UQ_Category_ParentCategoryId_Name");

            // Useful when retrieving child categories
            builder.HasIndex(x => x.ParentCategoryId)
                .HasDatabaseName("IX_Category_ParentCategoryId");

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
