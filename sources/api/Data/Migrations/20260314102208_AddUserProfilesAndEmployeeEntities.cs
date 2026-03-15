using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopNetApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserProfilesAndEmployeeEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "user_type",
                table: "users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "employee_profiles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    service = table.Column<string>(type: "text", nullable: false),
                    manager_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employee_profiles", x => x.id);
                    table.ForeignKey(
                        name: "FK_employee_profiles_employee_profiles_manager_id",
                        column: x => x.manager_id,
                        principalTable: "employee_profiles",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_employee_profiles_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_employee_profiles_manager_id",
                table: "employee_profiles",
                column: "manager_id");

            migrationBuilder.CreateIndex(
                name: "IX_employee_profiles_user_id",
                table: "employee_profiles",
                column: "user_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "employee_profiles");

            migrationBuilder.DropColumn(
                name: "user_type",
                table: "users");
        }
    }
}
