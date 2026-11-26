using ECommerce.Domain.InventoryManagement;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Persistence.Configurations
{
    public class StockReservationConfiguration : IEntityTypeConfiguration<StockReservation>
    {
        public void Configure(EntityTypeBuilder<StockReservation> builder)
        {
            builder.ToTable(
                "StockReservation",
                table =>
                {
                    table.HasCheckConstraint(
                        "CK_StockReservation_Quantity_Positive",
                        "[Quantity] > 0");

                    table.HasCheckConstraint(
                        "CK_StockReservation_Status_Valid",
                        "[Status] IN (1, 2, 3, 4)");
                });

            // Primary Key
            builder.HasKey(x => x.StockReservationId)
                .HasName("PK_StockReservation");

            builder.Property(x => x.StockReservationId)
                .ValueGeneratedOnAdd();

            // Inventory Relationship
            builder.Property(x => x.InventoryId)
                .IsRequired();

            builder.HasOne(x => x.Inventory)
                .WithMany(x => x.StockReservations)
                .HasForeignKey(x => x.InventoryId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName(
                    "FK_StockReservation_Inventory_InventoryId");

            // PaymentAttempt Relationship
            builder.Property(x => x.PaymentAttemptId)
                .IsRequired();

            builder.HasOne(x => x.PaymentAttempt)
                .WithMany(x => x.StockReservations)
                .HasForeignKey(x => x.PaymentAttemptId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName(
                    "FK_StockReservation_PaymentAttempt_PaymentAttemptId");

            // Quantity
            builder.Property(x => x.Quantity)
                .IsRequired();

            // Status
            builder.Property(x => x.Status)
                .IsRequired()
                .HasConversion<int>();

            // Expiration
            builder.Property(x => x.ExpiresAtUtc)
                .IsRequired()
                .HasColumnType("datetime2(3)");

            builder.Property(x => x.StatusChangedAtUtc)
                .IsRequired()
                .HasColumnType("datetime2(3)");

            // Optional reason when released
            builder.Property(x => x.ReleaseReason)
                .HasMaxLength(500);

            // Audit timestamps
            builder.Property(x => x.CreatedAtUtc)
                .IsRequired()
                .HasColumnType("datetime2(3)");

            builder.Property(x => x.UpdatedAtUtc)
                .IsRequired()
                .HasColumnType("datetime2(3)");

            // One reservation for the same inventory item
            // within the same payment attempt
            builder.HasIndex(x => new
            {
                x.PaymentAttemptId,
                x.InventoryId
            })
            .IsUnique()
            .HasDatabaseName(
                "UQ_StockReservation_PaymentAttemptId_InventoryId");

            // Useful for finding active/expired reservations
            builder.HasIndex(x => new
            {
                x.Status,
                x.ExpiresAtUtc
            })
            .HasDatabaseName(
                "IX_StockReservation_Status_ExpiresAtUtc");

            // Useful for inventory availability calculations
            builder.HasIndex(x => x.InventoryId)
                .HasDatabaseName(
                    "IX_StockReservation_InventoryId");
        }
    }
}
