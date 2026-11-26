using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddStockReservationsAndCODAllocations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CODStockAllocation",
                columns: table => new
                {
                    CODStockAllocationId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<long>(type: "bigint", nullable: false),
                    InventoryId = table.Column<long>(type: "bigint", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SecuredAtUtc = table.Column<DateTime>(type: "datetime2(3)", nullable: false),
                    ReleasedAtUtc = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    FinalizedAtUtc = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2(3)", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2(3)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CODStockAllocation", x => x.CODStockAllocationId);
                    table.CheckConstraint("CK_CODStockAllocation_Quantity_Positive", "[Quantity] > 0");
                    table.CheckConstraint("CK_CODStockAllocation_Status_Valid", "[Status] IN (1, 2, 3)");
                    table.ForeignKey(
                        name: "FK_CODStockAllocation_Inventory_InventoryId",
                        column: x => x.InventoryId,
                        principalTable: "Inventory",
                        principalColumn: "InventoryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CODStockAllocation_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StockReservation",
                columns: table => new
                {
                    StockReservationId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InventoryId = table.Column<long>(type: "bigint", nullable: false),
                    PaymentAttemptId = table.Column<long>(type: "bigint", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ExpiresAtUtc = table.Column<DateTime>(type: "datetime2(3)", nullable: false),
                    StatusChangedAtUtc = table.Column<DateTime>(type: "datetime2(3)", nullable: false),
                    ReleaseReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2(3)", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2(3)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockReservation", x => x.StockReservationId);
                    table.CheckConstraint("CK_StockReservation_Quantity_Positive", "[Quantity] > 0");
                    table.CheckConstraint("CK_StockReservation_Status_Valid", "[Status] IN (1, 2, 3, 4)");
                    table.ForeignKey(
                        name: "FK_StockReservation_Inventory_InventoryId",
                        column: x => x.InventoryId,
                        principalTable: "Inventory",
                        principalColumn: "InventoryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockReservation_PaymentAttempt_PaymentAttemptId",
                        column: x => x.PaymentAttemptId,
                        principalTable: "PaymentAttempt",
                        principalColumn: "PaymentAttemptId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CODStockAllocation_InventoryId",
                table: "CODStockAllocation",
                column: "InventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CODStockAllocation_Status",
                table: "CODStockAllocation",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "UQ_CODStockAllocation_OrderId_InventoryId",
                table: "CODStockAllocation",
                columns: new[] { "OrderId", "InventoryId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockReservation_InventoryId",
                table: "StockReservation",
                column: "InventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_StockReservation_Status_ExpiresAtUtc",
                table: "StockReservation",
                columns: new[] { "Status", "ExpiresAtUtc" });

            migrationBuilder.CreateIndex(
                name: "UQ_StockReservation_PaymentAttemptId_InventoryId",
                table: "StockReservation",
                columns: new[] { "PaymentAttemptId", "InventoryId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CODStockAllocation");

            migrationBuilder.DropTable(
                name: "StockReservation");
        }
    }
}
