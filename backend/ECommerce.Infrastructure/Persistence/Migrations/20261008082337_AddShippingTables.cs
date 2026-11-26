using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddShippingTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Shipment",
                columns: table => new
                {
                    ShipmentId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<long>(type: "bigint", nullable: false),
                    DeliveryStatus = table.Column<int>(type: "int", nullable: false),
                    TrackingReference = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ShippedAtUtc = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    DeliveredAtUtc = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2(3)", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2(3)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shipment", x => x.ShipmentId);
                    table.CheckConstraint("CK_Shipment_DeliveryStatus_Valid", "[DeliveryStatus] IN (1, 2, 3)");
                    table.ForeignKey(
                        name: "FK_Shipment_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShippingConfiguration",
                columns: table => new
                {
                    ShippingConfigurationId = table.Column<long>(type: "bigint", nullable: false),
                    ShippingMode = table.Column<int>(type: "int", nullable: false),
                    ShippingCharge = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2(3)", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2(3)", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingConfiguration", x => x.ShippingConfigurationId);
                    table.CheckConstraint("CK_ShippingConfiguration_ShippingCharge_NonNegative", "[ShippingCharge] >= 0");
                    table.CheckConstraint("CK_ShippingConfiguration_ShippingMode_Valid", "[ShippingMode] IN (1, 2)");
                    table.CheckConstraint("CK_ShippingConfiguration_Singleton", "[ShippingConfigurationId] = 1");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Shipment_DeliveryStatus",
                table: "Shipment",
                column: "DeliveryStatus");

            migrationBuilder.CreateIndex(
                name: "UQ_Shipment_OrderId",
                table: "Shipment",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_Shipment_TrackingReference",
                table: "Shipment",
                column: "TrackingReference",
                unique: true,
                filter: "[TrackingReference] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Shipment");

            migrationBuilder.DropTable(
                name: "ShippingConfiguration");
        }
    }
}
