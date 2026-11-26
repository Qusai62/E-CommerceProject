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
    public sealed class SupplierInvoiceConfiguration
     : IEntityTypeConfiguration<SupplierInvoice>
    {
        public void Configure(
            EntityTypeBuilder<SupplierInvoice> builder)
        {
            builder.ToTable("SupplierInvoices", table =>
            {
                table.HasCheckConstraint(
                    "CK_SupplierInvoice_TotalAmount",
                    "[TotalAmount] > 0");

                table.HasCheckConstraint(
                    "CK_SupplierInvoice_PaidAmount",
                    "[PaidAmount] >= 0 AND [PaidAmount] <= [TotalAmount]");
            });

            builder.HasKey(i => i.SupplierInvoiceId);

            builder.Property(i => i.SupplierInvoiceId)
                .ValueGeneratedOnAdd();

            builder.Property(i => i.InvoiceNumber)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(i => i.TotalAmount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(i => i.PaidAmount)
                .HasPrecision(18, 2)
                .HasDefaultValue(0m)
                .IsRequired();

            builder.Property(i => i.InvoiceDateUtc)
                .IsRequired();

            builder.Property(i => i.CreatedAtUtc)
                .IsRequired();

            builder.Property(i => i.UpdatedAtUtc)
                .IsRequired();

            builder.Property(i => i.RowVersion)
                .IsRowVersion();

            builder.Ignore(i => i.RemainingAmount);
            builder.Ignore(i => i.PaymentStatus);

            builder.HasOne(i => i.Supplier)
                .WithMany(s => s.Invoices)
                .HasForeignKey(i => i.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(i => i.Payments)
                .WithOne(p => p.SupplierInvoice)
                .HasForeignKey(p => p.SupplierInvoiceId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Navigation(i => i.Payments)
                .HasField("_payments")
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            // SupplierInvoice -> SupplierInvoiceItem
            builder.HasMany(i => i.Items)
                .WithOne(item => item.SupplierInvoice)
                .HasForeignKey(item => item.SupplierInvoiceId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // Use the private _items collection
            builder.Navigation(i => i.Items)
                .HasField("_items")
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasIndex(i => new
            {
                i.SupplierId,
                i.InvoiceNumber
            }).IsUnique();
        }
    }
}
