using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BinaryDecimalStore.Migrations
{
    /// <inheritdoc />
    public partial class AddCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categoreies",
                columns: new[] { "ID", "Description", "DisplayOrder", "name" },
                values: new object[,]
                {
                    { 1, "a Modern Phones From Apple", 1, "iphone" },
                    { 2, "a Modern HeadPhones From Apple", 3, "AirPods" },
                    { 3, "a Modern Laptop From Apple", 2, "MacBook" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categoreies",
                keyColumn: "ID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categoreies",
                keyColumn: "ID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categoreies",
                keyColumn: "ID",
                keyValue: 3);
        }
    }
}
