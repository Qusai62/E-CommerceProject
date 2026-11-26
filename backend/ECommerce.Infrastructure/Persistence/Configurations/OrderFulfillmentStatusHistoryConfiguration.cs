using ECommerce.Domain.Orders;
using ECommerce.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Persistence.Configurations
{
    public sealed class OrderFulfillmentStatusHistoryConfiguration
     : IEntityTypeConfiguration<OrderFulfillmentStatusHistory>
    {
        public void Configure(
            EntityTypeBuilder<OrderFulfillmentStatusHistory> builder)
        {
            // =====================================
            // Table and Check Constraints
            // =====================================

            builder.ToTable("OrderFulfillmentStatusHistory", table =>
            {
                table.HasCheckConstraint(
                    "CK_OrderFulfillmentStatusHistory_Status_Valid",
                    "[Status] IN (1, 2, 3, 4)");
            });

            // =====================================
            // Primary Key
            // =====================================

            builder.HasKey(x => x.OrderFulfillmentStatusHistoryId)
                .HasName("PK_OrderFulfillmentStatusHistory");

            builder.Property(x => x.OrderFulfillmentStatusHistoryId)
                .ValueGeneratedOnAdd();

            // =====================================
            // Order Relationship
            // =====================================

            builder.Property(x => x.OrderId)
                .IsRequired();

            builder.HasOne(x => x.Order)
                .WithMany(x => x.FulfillmentStatusHistory)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName(
                    "FK_OrderFulfillmentStatusHistory_Order_OrderId");

            // =====================================
            // Fulfillment Status
            // =====================================

            builder.Property(x => x.Status)
                .IsRequired()
                .HasConversion<int>();

            // =====================================
            // Change Timestamp
            // =====================================

            builder.Property(x => x.ChangedAtUtc)
                .IsRequired()
                .HasColumnType("datetime2(3)");

            // =====================================
            // Audit — Changed By
            // =====================================

            builder.Property(x => x.ChangedByApplicationUserId)
                .IsRequired(false);

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(x => x.ChangedByApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName(
                    "FK_OrderFulfillmentStatusHistory_ApplicationUser_ChangedByApplicationUserId");

            // =====================================
            // Audit — Reason
            // =====================================

            builder.Property(x => x.Reason)
                .IsRequired(false)
                .HasMaxLength(500);

            // =====================================
            // History Retrieval Index
            // =====================================

            builder.HasIndex(x => new
            {
                x.OrderId,
                x.ChangedAtUtc,
                x.OrderFulfillmentStatusHistoryId
            })
            .HasDatabaseName(
                "IX_OrderFulfillmentStatusHistory_OrderId_ChangedAtUtc");

            // =====================================
            // Actor Audit Index
            // =====================================

            builder.HasIndex(x => x.ChangedByApplicationUserId)
                .HasDatabaseName(
                    "IX_OrderFulfillmentStatusHistory_ChangedByApplicationUserId");

        }
    }
}
