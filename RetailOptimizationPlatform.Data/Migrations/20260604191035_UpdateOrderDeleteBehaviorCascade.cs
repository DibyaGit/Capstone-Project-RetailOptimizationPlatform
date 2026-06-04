using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RetailOptimizationPlatform.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOrderDeleteBehaviorCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Inventories_InventoryId",
                table: "Orders");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Inventories_InventoryId",
                table: "Orders",
                column: "InventoryId",
                principalTable: "Inventories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Inventories_InventoryId",
                table: "Orders");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Inventories_InventoryId",
                table: "Orders",
                column: "InventoryId",
                principalTable: "Inventories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
