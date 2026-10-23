using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    OrderId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    FulfillmentStatus = table.Column<int>(type: "int", nullable: false),
                    ShippingCharge = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    DeliveryRecipientName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DeliveryContactNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    DeliveryCity = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DeliveryNeighborhood = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    DeliveryStreet = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DeliveryBuildingInfo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DeliveryAdditionalInfo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PlacedAtUtc = table.Column<DateTime>(type: "datetime2(3)", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2(3)", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.OrderId);
                    table.CheckConstraint("CK_Order_FulfillmentStatus_Valid", "[FulfillmentStatus] IN (1, 2, 3, 4)");
                    table.CheckConstraint("CK_Order_ShippingCharge_NonNegative", "[ShippingCharge] >= 0");
                    table.CheckConstraint("CK_Order_TotalAmount_Positive", "[TotalAmount] > 0");
                    table.ForeignKey(
                        name: "FK_Order_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderFulfillmentStatusHistory",
                columns: table => new
                {
                    OrderFulfillmentStatusHistoryId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ChangedAtUtc = table.Column<DateTime>(type: "datetime2(3)", nullable: false),
                    ChangedByApplicationUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderFulfillmentStatusHistory", x => x.OrderFulfillmentStatusHistoryId);
                    table.CheckConstraint("CK_OrderFulfillmentStatusHistory_Status_Valid", "[Status] IN (1, 2, 3, 4)");
                    table.ForeignKey(
                        name: "FK_OrderFulfillmentStatusHistory_ApplicationUser_ChangedByApplicationUserId",
                        column: x => x.ChangedByApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderFulfillmentStatusHistory_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderItem",
                columns: table => new
                {
                    OrderItemId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<long>(type: "bigint", nullable: false),
                    ProductVariantId = table.Column<long>(type: "bigint", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    ProductNameSnapshot = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SkuSnapshot = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    VariantAttributesSnapshot = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2(3)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItem", x => x.OrderItemId);
                    table.CheckConstraint("CK_OrderItem_Quantity_Positive", "[Quantity] > 0");
                    table.CheckConstraint("CK_OrderItem_UnitPrice_Positive", "[UnitPrice] > 0");
                    table.ForeignKey(
                        name: "FK_OrderItem_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderItem_ProductVariant_ProductVariantId",
                        column: x => x.ProductVariantId,
                        principalTable: "ProductVariant",
                        principalColumn: "ProductVariantId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Order_CustomerId_PlacedAtUtc",
                table: "Order",
                columns: new[] { "CustomerId", "PlacedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_Order_FulfillmentStatus_PlacedAtUtc",
                table: "Order",
                columns: new[] { "FulfillmentStatus", "PlacedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "UQ_Order_OrderNumber",
                table: "Order",
                column: "OrderNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderFulfillmentStatusHistory_ChangedByApplicationUserId",
                table: "OrderFulfillmentStatusHistory",
                column: "ChangedByApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderFulfillmentStatusHistory_OrderId_ChangedAtUtc",
                table: "OrderFulfillmentStatusHistory",
                columns: new[] { "OrderId", "ChangedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_ProductVariantId",
                table: "OrderItem",
                column: "ProductVariantId");

            migrationBuilder.CreateIndex(
                name: "UQ_OrderItem_OrderId_ProductVariantId",
                table: "OrderItem",
                columns: new[] { "OrderId", "ProductVariantId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderFulfillmentStatusHistory");

            migrationBuilder.DropTable(
                name: "OrderItem");

            migrationBuilder.DropTable(
                name: "Order");
        }
    }
}
