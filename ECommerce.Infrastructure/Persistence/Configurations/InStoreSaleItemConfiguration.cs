using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Persistence.Configurations
{
    public sealed class InStoreSaleItemConfiguration
    : IEntityTypeConfiguration<InStoreSaleItem>
    {
        public void Configure(
            EntityTypeBuilder<InStoreSaleItem> builder)
        {
            builder.ToTable("InStoreSaleItems", table =>
            {
                table.HasCheckConstraint(
                    "CK_InStoreSaleItems_UnitPrice",
                    "[UnitPrice] > 0");

                table.HasCheckConstraint(
                    "CK_InStoreSaleItems_Quantity",
                    "[Quantity] > 0");

                table.HasCheckConstraint(
                    "CK_InStoreSaleItems_LineTotal",
                    "[LineTotal] > 0");

                table.HasCheckConstraint(
                    "CK_InStoreSaleItems_LineTotalCalculation",
                    "[LineTotal] = [UnitPrice] * [Quantity]");
            });

            // Primary key
            builder.HasKey(x => x.InStoreSaleItemId);

            builder.Property(x => x.InStoreSaleItemId)
                .ValueGeneratedOnAdd();

            // Foreign key
            builder.Property(x => x.InStoreSaleId)
                .IsRequired();

            // Item name
            builder.Property(x => x.ItemName)
                .IsRequired()
                .HasMaxLength(200);

            // Unit price
            builder.Property(x => x.UnitPrice)
                .HasPrecision(18, 2)
                .IsRequired();

            // Quantity
            builder.Property(x => x.Quantity)
                .IsRequired();

            // Line total
            builder.Property(x => x.LineTotal)
                .HasPrecision(18, 2)
                .IsRequired();

            // Foreign key index
            builder.HasIndex(x => x.InStoreSaleId);
        }

    }
}
