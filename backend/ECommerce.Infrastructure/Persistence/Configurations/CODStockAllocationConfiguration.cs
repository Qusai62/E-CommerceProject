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
    public class CODStockAllocationConfiguration : IEntityTypeConfiguration<CODStockAllocation>
    {
        public void Configure(EntityTypeBuilder<CODStockAllocation> builder)
        {
            builder.ToTable(
                "CODStockAllocation",
                table =>
                {
                    table.HasCheckConstraint(
                        "CK_CODStockAllocation_Quantity_Positive",
                        "[Quantity] > 0");

                    table.HasCheckConstraint(
                        "CK_CODStockAllocation_Status_Valid",
                        "[Status] IN (1, 2, 3)");
                });

            // Primary Key
            builder.HasKey(x => x.CODStockAllocationId)
                .HasName("PK_CODStockAllocation");

            builder.Property(x => x.CODStockAllocationId)
                .ValueGeneratedOnAdd();

            // Order Relationship
            builder.Property(x => x.OrderId)
                .IsRequired();

            builder.HasOne(x => x.Order)
                .WithMany(x => x.CODStockAllocations)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName(
                    "FK_CODStockAllocation_Order_OrderId");

            // Inventory Relationship
            builder.Property(x => x.InventoryId)
                .IsRequired();

            builder.HasOne(x => x.Inventory)
                .WithMany(x => x.CODStockAllocations)
                .HasForeignKey(x => x.InventoryId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName(
                    "FK_CODStockAllocation_Inventory_InventoryId");

            // Quantity
            builder.Property(x => x.Quantity)
                .IsRequired();

            // Status
            builder.Property(x => x.Status)
                .IsRequired()
                .HasConversion<int>();

            // Lifecycle timestamps
            builder.Property(x => x.SecuredAtUtc)
                .IsRequired()
                .HasColumnType("datetime2(3)");

            builder.Property(x => x.ReleasedAtUtc)
                .HasColumnType("datetime2(3)");

            builder.Property(x => x.FinalizedAtUtc)
                .HasColumnType("datetime2(3)");

            // Audit timestamps
            builder.Property(x => x.CreatedAtUtc)
                .IsRequired()
                .HasColumnType("datetime2(3)");

            builder.Property(x => x.UpdatedAtUtc)
                .IsRequired()
                .HasColumnType("datetime2(3)");

            // One allocation per Inventory item inside an Order
            builder.HasIndex(x => new
            {
                x.OrderId,
                x.InventoryId
            })
            .IsUnique()
            .HasDatabaseName(
                "UQ_CODStockAllocation_OrderId_InventoryId");

            // Useful for inventory availability calculations
            builder.HasIndex(x => x.InventoryId)
                .HasDatabaseName(
                    "IX_CODStockAllocation_InventoryId");

            // Useful for finding secured allocations
            builder.HasIndex(x => x.Status)
                .HasDatabaseName(
                    "IX_CODStockAllocation_Status");
        }
    }
}
