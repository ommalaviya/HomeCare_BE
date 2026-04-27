using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shared.HomeCare.Migrations
{
    public partial class AddDurationInBooking : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "duration",
                schema: "HomeCare",
                table: "bookings",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "duration",
                schema: "HomeCare",
                table: "bookings");
        }
    }
}