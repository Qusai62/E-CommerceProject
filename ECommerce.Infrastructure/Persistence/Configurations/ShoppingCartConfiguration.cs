using ECommerce.Domain.Cart;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Persistence.Configurations
{
    public class ShoppingCartConfiguration : IEntityTypeConfiguration<ShoppingCart>
    {
        public void Configure(EntityTypeBuilder<ShoppingCart> builder)
        {
            builder.ToTable("ShoppingCart");

            // Primary Key
            builder.HasKey(x => x.ShoppingCartId)
                .HasName("PK_ShoppingCart");

            builder.Property(x => x.ShoppingCartId)
                .ValueGeneratedOnAdd();

            // Customer Relationship
            builder.Property(x => x.CustomerId)
                .IsRequired();

            builder.HasOne(x => x.Customer)
                .WithOne(x => x.ShoppingCart)
                .HasForeignKey<ShoppingCart>(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ShoppingCart_Customer_CustomerId");

            // Enforce one ShoppingCart per Customer
            builder.HasIndex(x => x.CustomerId)
                .IsUnique()
                .HasDatabaseName("UQ_ShoppingCart_CustomerId");

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
