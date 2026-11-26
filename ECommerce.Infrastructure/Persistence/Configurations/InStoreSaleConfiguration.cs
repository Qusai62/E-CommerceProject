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
    public sealed class InStoreSaleConfiguration
    : IEntityTypeConfiguration<InStoreSale>
    {
        public void Configure(
            EntityTypeBuilder<InStoreSale> builder)
        {
            builder.ToTable("InStoreSales", table =>
            {
                table.HasCheckConstraint(
                    "CK_InStoreSales_TotalAmount",
                    "[TotalAmount] > 0");

                table.HasCheckConstraint(
                    "CK_InStoreSales_Status",
                    "[Status] IN (1, 2)");
            });

            // Primary key
            builder.HasKey(x => x.InStoreSaleId);

            builder.Property(x => x.InStoreSaleId)
                .ValueGeneratedOnAdd();

            // Unique sale number
            builder.Property(x => x.SaleNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(x => x.SaleNumber)
                .IsUnique();

            // Sale total
            builder.Property(x => x.TotalAmount)
                .HasPrecision(18, 2)
                .IsRequired();

            // Status
            builder.Property(x => x.Status)
                .HasConversion<int>()
                .IsRequired();

            // Idempotency identifier
            builder.Property(x => x.SubmissionId)
                .IsRequired();

            builder.HasIndex(x => x.SubmissionId)
                .IsUnique();

            // Dates
            builder.Property(x => x.SoldAtUtc)
                .HasColumnType("datetime2")
                .IsRequired();

            builder.Property(x => x.VoidedAtUtc)
                .HasColumnType("datetime2")
                .IsRequired(false);


            builder.Property(x => x.SubmissionFingerprint)
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(s => s.UpdatedAtUtc)
                .IsRequired();

            builder.Property(s => s.RowVersion)
                .IsRowVersion();


            // Sales history / reports
            builder.HasIndex(x => x.SoldAtUtc);

            // One sale -> many items
            builder.HasMany(x => x.Items)
                .WithOne(x => x.InStoreSale)
                .HasForeignKey(x => x.InStoreSaleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
