using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shared.HomeCare.Migrations
{
    /// <inheritdoc />
    public partial class CancleReasonToBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "cancellation_reason",
                schema: "HomeCare",
                table: "bookings",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                schema: "HomeCare",
                table: "bookings",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "created_by",
                schema: "HomeCare",
                table: "bookings",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                schema: "HomeCare",
                table: "bookings",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "modified_at",
                schema: "HomeCare",
                table: "bookings",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "modified_by",
                schema: "HomeCare",
                table: "bookings",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "cancellation_reason",
                schema: "HomeCare",
                table: "bookings");

            migrationBuilder.DropColumn(
                name: "created_at",
                schema: "HomeCare",
                table: "bookings");

            migrationBuilder.DropColumn(
                name: "created_by",
                schema: "HomeCare",
                table: "bookings");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                schema: "HomeCare",
                table: "bookings");

            migrationBuilder.DropColumn(
                name: "modified_at",
                schema: "HomeCare",
                table: "bookings");

            migrationBuilder.DropColumn(
                name: "modified_by",
                schema: "HomeCare",
                table: "bookings");
        }
    }
}
