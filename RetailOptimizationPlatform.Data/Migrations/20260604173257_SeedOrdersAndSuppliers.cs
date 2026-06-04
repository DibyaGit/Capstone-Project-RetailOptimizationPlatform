using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RetailOptimizationPlatform.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedOrdersAndSuppliers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Suppliers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ContactPerson = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    State = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    LeadTimeDays = table.Column<int>(type: "int", nullable: false),
                    PaymentTerms = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreditLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Rating = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suppliers", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "Id", "CustomerName", "InventoryId", "OrderDate", "TotalAmount" },
                values: new object[,]
                {
                    { 1, "John Doe", 1, new DateTime(2026, 3, 15, 0, 0, 0, 0, DateTimeKind.Utc), 51.98m },
                    { 2, "Jane Smith", 2, new DateTime(2026, 4, 10, 0, 0, 0, 0, DateTimeKind.Utc), 89.50m },
                    { 3, "Bob Johnson", 3, new DateTime(2026, 5, 5, 0, 0, 0, 0, DateTimeKind.Utc), 299.99m },
                    { 4, "Alice Brown", 4, new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), 150.00m }
                });

            migrationBuilder.InsertData(
                table: "Suppliers",
                columns: new[] { "Id", "City", "ContactPerson", "Country", "CreatedDate", "CreditLimit", "Email", "IsActive", "LastUpdated", "LeadTimeDays", "Name", "Notes", "PaymentTerms", "PhoneNumber", "PostalCode", "Rating", "State" },
                values: new object[,]
                {
                    { 1, "Newark", "Markus Senn", "USA", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 50000m, "markus@logitech.com", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 5, "Logitech Corp", null, "Net 30", "+1-555-0199", null, 4.8m, "NJ" },
                    { 2, "Hong Kong", "Nick Chen", "Hong Kong", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 30000m, "nick@keychron.com", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 12, "Keychron Inc", null, "Net 15", "+852-2345-6789", null, 4.5m, "" },
                    { 3, "Round Rock", "Sarah Connor", "USA", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 100000m, "sarah@dell.com", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 7, "Dell Technologies", null, "Net 45", "+1-800-456-3355", null, 4.7m, "TX" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Suppliers");

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
