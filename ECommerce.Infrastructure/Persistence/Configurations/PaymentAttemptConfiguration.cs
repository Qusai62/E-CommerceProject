using ECommerce.Domain.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Persistence.Configurations
{
    public class PaymentAttemptConfiguration : IEntityTypeConfiguration<PaymentAttempt>
    {
        public void Configure(EntityTypeBuilder<PaymentAttempt> builder)
        {
            builder.ToTable(
                "PaymentAttempt",
                table =>
                {
                    table.HasCheckConstraint(
                        "CK_PaymentAttempt_AttemptNumber_Positive",
                        "[AttemptNumber] > 0");

                    table.HasCheckConstraint(
                        "CK_PaymentAttempt_Amount_Positive",
                        "[Amount] > 0");

                    table.HasCheckConstraint(
                        "CK_PaymentAttempt_Status_Valid",
                        "[Status] IN (1, 2, 3)");
                });

            // Primary Key
            builder.HasKey(x => x.PaymentAttemptId)
                .HasName("PK_PaymentAttempt");

            builder.Property(x => x.PaymentAttemptId)
                .ValueGeneratedOnAdd();

            // Payment Relationship
            builder.Property(x => x.PaymentId)
                .IsRequired();

            builder.HasOne(x => x.Payment)
                .WithMany(x => x.PaymentAttempts)
                .HasForeignKey(x => x.PaymentId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName(
                    "FK_PaymentAttempt_Payment_PaymentId");

            // Attempt number
            builder.Property(x => x.AttemptNumber)
                .IsRequired();

            // Prevent duplicate attempt numbers for one Payment
            builder.HasIndex(x => new
            {
                x.PaymentId,
                x.AttemptNumber
            })
            .IsUnique()
            .HasDatabaseName(
                "UQ_PaymentAttempt_PaymentId_AttemptNumber");

            // Amount
            builder.Property(x => x.Amount)
                .IsRequired()
                .HasPrecision(18, 3);

            // Status
            builder.Property(x => x.Status)
                .IsRequired()
                .HasConversion<int>();

            // Our internal unique reference
            builder.Property(x => x.PaymentReference)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(x => x.PaymentReference)
                .IsUnique()
                .HasDatabaseName(
                    "UQ_PaymentAttempt_PaymentReference");

            // Reference returned by payment provider
            builder.Property(x => x.ProviderReference)
                .HasMaxLength(200);

            // Failure information
            builder.Property(x => x.FailureCode)
                .HasMaxLength(100);

            builder.Property(x => x.FailureReason)
                .HasMaxLength(500);

            // Timestamps
            builder.Property(x => x.CreatedAtUtc)
                .IsRequired()
                .HasColumnType("datetime2(3)");

            builder.Property(x => x.CompletedAtUtc)
                .HasColumnType("datetime2(3)");
        }
    }
}
