using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopNetApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixWarehouseAddressOptional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_warehouses_addresses_address_id",
                table: "warehouses");

            migrationBuilder.AlterColumn<Guid>(
                name: "address_id",
                table: "warehouses",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_warehouses_addresses_address_id",
                table: "warehouses",
                column: "address_id",
                principalTable: "addresses",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_warehouses_addresses_address_id",
                table: "warehouses");

            migrationBuilder.AlterColumn<Guid>(
                name: "address_id",
                table: "warehouses",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_warehouses_addresses_address_id",
                table: "warehouses",
                column: "address_id",
                principalTable: "addresses",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
