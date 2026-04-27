using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shared.HomeCare.Migrations
{
    /// <inheritdoc />
    public partial class StripeTransactionIDColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "stripe_payment_intent_id",
                schema: "HomeCare",
                table: "transactions",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "stripe_payment_intent_id",
                schema: "HomeCare",
                table: "transactions");
        }
    }
}
