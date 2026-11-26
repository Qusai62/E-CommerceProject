using ECommerce.Domain.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
namespace ECommerce.Infrastructure.Persistence.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable(
                "Payment",
                table =>
                {
                    table.HasCheckConstraint(
                        "CK_Payment_ExpectedAmount_Positive",
                        "[ExpectedAmount] > 0");

                    table.HasCheckConstraint(
                        "CK_Payment_Method_Valid",
                        "[Method] IN (1, 2)");

                    table.HasCheckConstraint(
                        "CK_Payment_Status_Valid",
                        "[Status] IN (1, 2)");
                });

            // Primary Key
            builder.HasKey(x => x.PaymentId)
                .HasName("PK_Payment");

            builder.Property(x => x.PaymentId)
                .ValueGeneratedOnAdd();

            // Order Relationship
            builder.Property(x => x.OrderId)
                .IsRequired();

            builder.HasOne(x => x.Order)
                .WithOne(x => x.Payment)
                .HasForeignKey<Payment>(x => x.OrderId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName(
                    "FK_Payment_Order_OrderId");

            // One Payment per Order
            builder.HasIndex(x => x.OrderId)
                .IsUnique()
                .HasDatabaseName("UQ_Payment_OrderId");

            // Payment Method
            builder.Property(x => x.Method)
                .IsRequired()
                .HasConversion<int>();

            // Payment Status
            builder.Property(x => x.Status)
                .IsRequired()
                .HasConversion<int>();

            // Expected Amount
            builder.Property(x => x.ExpectedAmount)
                .IsRequired()
                .HasPrecision(18, 3);

            // Paid timestamp
            builder.Property(x => x.PaidAtUtc)
                .HasColumnType("datetime2(3)");

            // Audit timestamps
            builder.Property(x => x.CreatedAtUtc)
                .IsRequired()
                .HasColumnType("datetime2(3)");

            builder.Property(x => x.UpdatedAtUtc)
                .IsRequired()
                .HasColumnType("datetime2(3)");

            // Optimistic concurrency
            builder.Property<byte[]>("RowVersion")
                .IsRowVersion()
                .IsConcurrencyToken();

            // Useful for querying payments by status
            builder.HasIndex(x => x.Status)
                .HasDatabaseName("IX_Payment_Status");
        }
    }
}
