using ECommerce.Domain.Supplier_Invoice___Payment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Persistence.Configurations
{
    public sealed class SupplierInvoiceItemConfiguration
     : IEntityTypeConfiguration<SupplierInvoiceItem>
    {
        public void Configure(
            EntityTypeBuilder<SupplierInvoiceItem> builder)
        {
            // 1. Table name
            builder.ToTable("SupplierInvoiceItems", table =>
            {
                table.HasCheckConstraint(
                    "CK_SupplierInvoiceItem_Quantity",
                    "[Quantity] > 0");

                table.HasCheckConstraint(
                    "CK_SupplierInvoiceItem_UnitPrice",
                    "[UnitPrice] > 0");
            });

            // 2. Primary key
            builder.HasKey(i => i.SupplierInvoiceItemId);

            builder.Property(i => i.SupplierInvoiceItemId)
                .ValueGeneratedOnAdd();

            // 3. Product name
            builder.Property(i => i.ProductName)
                .IsRequired()
                .HasMaxLength(200);

            // 4. Quantity
            builder.Property(i => i.Quantity)
                .IsRequired();

            // 5. Purchase unit price
            builder.Property(i => i.UnitPrice)
                .HasPrecision(18, 2)
                .IsRequired();

            // 6. Calculated property
            builder.Ignore(i => i.LineTotal);

            // 7. Index for invoice item lookup
            builder.HasIndex(i => i.SupplierInvoiceId);
        }
    }
}
