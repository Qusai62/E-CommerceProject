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
    public sealed class SupplierPaymentConfiguration
    : IEntityTypeConfiguration<SupplierPayment>
    {
        public void Configure(
            EntityTypeBuilder<SupplierPayment> builder)
        {
            builder.ToTable("SupplierPayments", table =>
            {
                table.HasCheckConstraint(
                    "CK_SupplierPayment_Amount",
                    "[Amount] > 0");
            });

            builder.HasKey(p => p.SupplierPaymentId);

            builder.Property(p => p.SupplierPaymentId)
                .ValueGeneratedOnAdd();

            builder.Property(p => p.Amount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(p => p.PaidAtUtc)
                .IsRequired();

            builder.Property(p => p.RecordedAtUtc)
                .IsRequired();

            builder.HasOne(p => p.SupplierInvoice)
                .WithMany(i => i.Payments)
                .HasForeignKey(p => p.SupplierInvoiceId)
                .OnDelete(DeleteBehavior.Restrict);

            // Unique identifier for the payment submission.
            builder.Property(p => p.SubmissionId)
                .IsRequired();

            // Prevent two payments from using the same submission ID.
            builder.HasIndex(p => p.SubmissionId)
                .IsUnique()
                .HasDatabaseName("UX_SupplierPayments_SubmissionId");

            builder.HasIndex(p => new
            {
                p.SupplierInvoiceId,
                p.PaidAtUtc
            });
        }
    }
}
