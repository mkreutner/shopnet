using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopNetApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixMovementTypeToEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "movement_type",
                table: "stock_movements",
                newName: "type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "type",
                table: "stock_movements",
                newName: "movement_type");
        }
    }
}
