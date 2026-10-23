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
    public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            builder.ToTable(
                "CartItem",
                table =>
                {
                    table.HasCheckConstraint(
                        "CK_CartItem_Quantity_Positive",
                        "[Quantity] > 0");
                });

            // Primary Key
            builder.HasKey(x => x.CartItemId)
                .HasName("PK_CartItem");

            builder.Property(x => x.CartItemId)
                .ValueGeneratedOnAdd();

            // ShoppingCart Relationship
            builder.Property(x => x.ShoppingCartId)
                .IsRequired();

            builder.HasOne(x => x.ShoppingCart)
                .WithMany(x => x.CartItems)
                .HasForeignKey(x => x.ShoppingCartId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName(
                    "FK_CartItem_ShoppingCart_ShoppingCartId");

            // ProductVariant Relationship
            builder.Property(x => x.ProductVariantId)
                .IsRequired();

            builder.HasOne(x => x.ProductVariant)
                .WithMany(x => x.CartItems)
                .HasForeignKey(x => x.ProductVariantId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName(
                    "FK_CartItem_ProductVariant_ProductVariantId");

            // Quantity
            builder.Property(x => x.Quantity)
                .IsRequired();

            // Prevent duplicate Variant rows inside the same Cart
            builder.HasIndex(x => new
            {
                x.ShoppingCartId,
                x.ProductVariantId
            })
            .IsUnique()
            .HasDatabaseName(
                "UQ_CartItem_ShoppingCartId_ProductVariantId");

            // Useful when querying carts that contain a Variant
            builder.HasIndex(x => x.ProductVariantId)
                .HasDatabaseName(
                    "IX_CartItem_ProductVariantId");

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
