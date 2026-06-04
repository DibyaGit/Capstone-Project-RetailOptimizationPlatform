using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RetailOptimizationPlatform.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDatabaseArchitecture : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CustomerName",
                table: "Orders",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "InventoryId",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Sku",
                table: "Inventories",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ItemName",
                table: "Inventories",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.InsertData(
                table: "Inventories",
                columns: new[] { "Id", "ItemName", "Price", "Sku", "StockQuantity" },
                values: new object[,]
                {
                    { 1, "Wireless Mouse", 25.99m, "MOUSE-001", 50 },
                    { 2, "Mechanical Keyboard", 89.50m, "KEY-002", 30 },
                    { 3, "USB-C Monitor", 299.99m, "MON-003", 15 },
                    { 4, "Ergonomic Chair", 150.00m, "FURN-004", 5 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_InventoryId",
                table: "Orders",
                column: "InventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_Sku",
                table: "Inventories",
                column: "Sku",
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Inventory_Price",
                table: "Inventories",
                sql: "[Price] >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Inventory_StockQuantity",
                table: "Inventories",
                sql: "[StockQuantity] >= 0");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Inventories_InventoryId",
                table: "Orders",
                column: "InventoryId",
                principalTable: "Inventories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Inventories_InventoryId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_InventoryId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Inventories_Sku",
                table: "Inventories");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Inventory_Price",
                table: "Inventories");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Inventory_StockQuantity",
                table: "Inventories");

            migrationBuilder.DeleteData(
                table: "Inventories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Inventories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Inventories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Inventories",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DropColumn(
                name: "InventoryId",
                table: "Orders");

            migrationBuilder.AlterColumn<string>(
                name: "CustomerName",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Sku",
                table: "Inventories",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "ItemName",
                table: "Inventories",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);
        }
    }
}
