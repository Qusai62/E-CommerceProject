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
    public class OrderFulfillmentStatusHistoryConfiguration : IEntityTypeConfiguration<OrderFulfillmentStatusHistory>
    {
        public void Configure(
        EntityTypeBuilder<OrderFulfillmentStatusHistory> builder)
        {
            builder.ToTable(
                "OrderFulfillmentStatusHistory",
                table =>
                {
                    table.HasCheckConstraint(
                        "CK_OrderFulfillmentStatusHistory_Status_Valid",
                        "[Status] IN (1, 2, 3, 4)");
                });

            // Primary Key
            builder.HasKey(x => x.OrderFulfillmentStatusHistoryId)
                .HasName("PK_OrderFulfillmentStatusHistory");

            builder.Property(x => x.OrderFulfillmentStatusHistoryId)
                .ValueGeneratedOnAdd();

            // Order Relationship
            builder.Property(x => x.OrderId)
                .IsRequired();

            builder.HasOne(x => x.Order)
                .WithMany(x => x.FulfillmentStatusHistory)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName(
                    "FK_OrderFulfillmentStatusHistory_Order_OrderId");

            // Status
            builder.Property(x => x.Status)
                .IsRequired()
                .HasConversion<int>();

            // When the status changed
            builder.Property(x => x.ChangedAtUtc)
                .IsRequired()
                .HasColumnType("datetime2(3)");

            // Optional user who performed the change
            builder.Property(x => x.ChangedByApplicationUserId)
                .IsRequired(false);

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(x => x.ChangedByApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName(
                    "FK_OrderFulfillmentStatusHistory_ApplicationUser_ChangedByApplicationUserId");

            // Optional reason
            builder.Property(x => x.Reason)
                .HasMaxLength(500);

            // Common history query
            builder.HasIndex(x => new
            {
                x.OrderId,
                x.ChangedAtUtc
            })
            .HasDatabaseName(
                "IX_OrderFulfillmentStatusHistory_OrderId_ChangedAtUtc");

            // Useful when auditing actions performed by a user
            builder.HasIndex(x => x.ChangedByApplicationUserId)
                .HasDatabaseName(
                    "IX_OrderFulfillmentStatusHistory_ChangedByApplicationUserId");
        }
    }
}
