using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shared.HomeCare.Migrations
{
    /// <inheritdoc />
    public partial class AddUserColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "mobile_number",
                schema: "HomeCare",
                table: "users",
                type: "character varying(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "status",
                schema: "HomeCare",
                table: "users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                schema: "HomeCare",
                table: "users",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "mobile_number",
                schema: "HomeCare",
                table: "users");

            migrationBuilder.DropColumn(
                name: "status",
                schema: "HomeCare",
                table: "users");

            migrationBuilder.DropColumn(
                name: "updated_at",
                schema: "HomeCare",
                table: "users");
        }
    }
}
