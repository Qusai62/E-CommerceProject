using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddInStoreSalesTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InStoreSales",
                columns: table => new
                {
                    InStoreSaleId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SaleNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SubmissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SoldAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VoidedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InStoreSales", x => x.InStoreSaleId);
                    table.CheckConstraint("CK_InStoreSales_Status", "[Status] IN (1, 2)");
                    table.CheckConstraint("CK_InStoreSales_TotalAmount", "[TotalAmount] > 0");
                });

            migrationBuilder.CreateTable(
                name: "InStoreSaleItems",
                columns: table => new
                {
                    InStoreSaleItemId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InStoreSaleId = table.Column<long>(type: "bigint", nullable: false),
                    ItemName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    LineTotal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InStoreSaleItems", x => x.InStoreSaleItemId);
                    table.CheckConstraint("CK_InStoreSaleItems_LineTotal", "[LineTotal] > 0");
                    table.CheckConstraint("CK_InStoreSaleItems_LineTotalCalculation", "[LineTotal] = [UnitPrice] * [Quantity]");
                    table.CheckConstraint("CK_InStoreSaleItems_Quantity", "[Quantity] > 0");
                    table.CheckConstraint("CK_InStoreSaleItems_UnitPrice", "[UnitPrice] > 0");
                    table.ForeignKey(
                        name: "FK_InStoreSaleItems_InStoreSales_InStoreSaleId",
                        column: x => x.InStoreSaleId,
                        principalTable: "InStoreSales",
                        principalColumn: "InStoreSaleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InStoreSaleItems_InStoreSaleId",
                table: "InStoreSaleItems",
                column: "InStoreSaleId");

            migrationBuilder.CreateIndex(
                name: "IX_InStoreSales_SaleNumber",
                table: "InStoreSales",
                column: "SaleNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InStoreSales_SoldAtUtc",
                table: "InStoreSales",
                column: "SoldAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_InStoreSales_SubmissionId",
                table: "InStoreSales",
                column: "SubmissionId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InStoreSaleItems");

            migrationBuilder.DropTable(
                name: "InStoreSales");
        }
    }
}
