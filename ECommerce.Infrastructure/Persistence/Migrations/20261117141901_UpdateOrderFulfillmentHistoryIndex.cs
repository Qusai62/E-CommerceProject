using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOrderFulfillmentHistoryIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrderFulfillmentStatusHistory_OrderId_ChangedAtUtc",
                table: "OrderFulfillmentStatusHistory");

            migrationBuilder.CreateIndex(
                name: "IX_OrderFulfillmentStatusHistory_OrderId_ChangedAtUtc",
                table: "OrderFulfillmentStatusHistory",
                columns: new[] { "OrderId", "ChangedAtUtc", "OrderFulfillmentStatusHistoryId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrderFulfillmentStatusHistory_OrderId_ChangedAtUtc",
                table: "OrderFulfillmentStatusHistory");

            migrationBuilder.CreateIndex(
                name: "IX_OrderFulfillmentStatusHistory_OrderId_ChangedAtUtc",
                table: "OrderFulfillmentStatusHistory",
                columns: new[] { "OrderId", "ChangedAtUtc" });
        }
    }
}
