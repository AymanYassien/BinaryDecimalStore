using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BinaryDecimalStore.Migrations
{
    /// <inheritdoc />
    public partial class addConstraintOnDisplayOrderToBeUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Categoreies_DisplayOrder",
                table: "Categoreies",
                column: "DisplayOrder",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Categoreies_DisplayOrder",
                table: "Categoreies");
        }
    }
}
