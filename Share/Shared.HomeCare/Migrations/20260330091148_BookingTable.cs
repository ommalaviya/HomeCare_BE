using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Shared.HomeCare.Migrations
{
    /// <inheritdoc />
    public partial class BookingTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "user_id",
                schema: "HomeCare",
                table: "users",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "otp_id",
                schema: "HomeCare",
                table: "user_otps",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "address_id",
                schema: "HomeCare",
                table: "user_addresses",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "sub_category_id",
                schema: "HomeCare",
                table: "sub_categories",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "service_image_id",
                schema: "HomeCare",
                table: "services_images",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "service_id",
                schema: "HomeCare",
                table: "services",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "service_id",
                schema: "HomeCare",
                table: "service_types",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "sp_id",
                schema: "HomeCare",
                table: "service_partners",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "spsk_id",
                schema: "HomeCare",
                table: "service_partner_skills",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "spso_id",
                schema: "HomeCare",
                table: "service_partner_services_offered",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "spl_id",
                schema: "HomeCare",
                table: "service_partner_languages",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "spex_id",
                schema: "HomeCare",
                table: "service_partner_experiences",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "spe_id",
                schema: "HomeCare",
                table: "service_partner_educations",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "spa_id",
                schema: "HomeCare",
                table: "service_partner_attachments",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "service_inclusion_exclusion_id",
                schema: "HomeCare",
                table: "service_inclusions_exclusions",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "offer_id",
                schema: "HomeCare",
                table: "offers",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "language_id",
                schema: "HomeCare",
                table: "languages",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "category_id",
                schema: "HomeCare",
                table: "categories",
                newName: "id");

            migrationBuilder.AlterColumn<string>(
                name: "user_name",
                schema: "HomeCare",
                table: "users",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "user_email",
                schema: "HomeCare",
                table: "users",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "mobile_number",
                schema: "HomeCare",
                table: "users",
                type: "character varying(15)",
                maxLength: 15,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "refresh_token_hash",
                schema: "HomeCare",
                table: "user_otps",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "otp_code",
                schema: "HomeCare",
                table: "user_otps",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<decimal>(
                name: "commission",
                schema: "HomeCare",
                table: "services",
                type: "numeric(5,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddColumn<string>(
                name: "description",
                schema: "HomeCare",
                table: "services",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "bookings",
                schema: "HomeCare",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    service_id = table.Column<int>(type: "integer", nullable: false),
                    service_type_id = table.Column<int>(type: "integer", nullable: false),
                    assigned_partner_id = table.Column<int>(type: "integer", nullable: true),
                    address_id = table.Column<int>(type: "integer", nullable: false),
                    booking_date = table.Column<DateOnly>(type: "date", nullable: false),
                    booking_time = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    offer_id = table.Column<int>(type: "integer", nullable: true),
                    payment_method = table.Column<string>(type: "text", nullable: false),
                    booking_amount = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false, defaultValue: "Pending"),
                    payment_status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bookings", x => x.id);
                    table.ForeignKey(
                        name: "FK_bookings_offers_offer_id",
                        column: x => x.offer_id,
                        principalSchema: "HomeCare",
                        principalTable: "offers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_bookings_service_partners_assigned_partner_id",
                        column: x => x.assigned_partner_id,
                        principalSchema: "HomeCare",
                        principalTable: "service_partners",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_bookings_service_types_service_type_id",
                        column: x => x.service_type_id,
                        principalSchema: "HomeCare",
                        principalTable: "service_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_bookings_services_service_id",
                        column: x => x.service_id,
                        principalSchema: "HomeCare",
                        principalTable: "services",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_bookings_user_addresses_address_id",
                        column: x => x.address_id,
                        principalSchema: "HomeCare",
                        principalTable: "user_addresses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_bookings_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "HomeCare",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "support_tickets",
                schema: "HomeCare",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    contact_number = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    submitted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_support_tickets", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "coupon_usages",
                schema: "HomeCare",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    coupon_id = table.Column<int>(type: "integer", nullable: false),
                    booking_id = table.Column<int>(type: "integer", nullable: false),
                    used_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_coupon_usages", x => x.id);
                    table.ForeignKey(
                        name: "FK_coupon_usages_bookings_booking_id",
                        column: x => x.booking_id,
                        principalSchema: "HomeCare",
                        principalTable: "bookings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_coupon_usages_offers_coupon_id",
                        column: x => x.coupon_id,
                        principalSchema: "HomeCare",
                        principalTable: "offers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_coupon_usages_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "HomeCare",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "transactions",
                schema: "HomeCare",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    transaction_uuid = table.Column<Guid>(type: "uuid", nullable: false),
                    booking_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    service_id = table.Column<int>(type: "integer", nullable: false),
                    transaction_amount = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    payment_method = table.Column<string>(type: "text", nullable: false),
                    transaction_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    payment_status = table.Column<string>(type: "text", nullable: false),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_by = table.Column<int>(type: "integer", nullable: true),
                    modified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transactions", x => x.id);
                    table.ForeignKey(
                        name: "FK_transactions_bookings_booking_id",
                        column: x => x.booking_id,
                        principalSchema: "HomeCare",
                        principalTable: "bookings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_transactions_services_service_id",
                        column: x => x.service_id,
                        principalSchema: "HomeCare",
                        principalTable: "services",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_transactions_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "HomeCare",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_bookings_address_id",
                schema: "HomeCare",
                table: "bookings",
                column: "address_id");

            migrationBuilder.CreateIndex(
                name: "IX_bookings_assigned_partner_id",
                schema: "HomeCare",
                table: "bookings",
                column: "assigned_partner_id");

            migrationBuilder.CreateIndex(
                name: "IX_bookings_booking_date_booking_time",
                schema: "HomeCare",
                table: "bookings",
                columns: new[] { "booking_date", "booking_time" });

            migrationBuilder.CreateIndex(
                name: "IX_bookings_offer_id",
                schema: "HomeCare",
                table: "bookings",
                column: "offer_id");

            migrationBuilder.CreateIndex(
                name: "IX_bookings_service_id",
                schema: "HomeCare",
                table: "bookings",
                column: "service_id");

            migrationBuilder.CreateIndex(
                name: "IX_bookings_service_type_id",
                schema: "HomeCare",
                table: "bookings",
                column: "service_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_bookings_status",
                schema: "HomeCare",
                table: "bookings",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_bookings_user_id",
                schema: "HomeCare",
                table: "bookings",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_coupon_usages_booking_id",
                schema: "HomeCare",
                table: "coupon_usages",
                column: "booking_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_coupon_usages_coupon_id",
                schema: "HomeCare",
                table: "coupon_usages",
                column: "coupon_id");

            migrationBuilder.CreateIndex(
                name: "IX_coupon_usages_user_id_coupon_id_booking_id",
                schema: "HomeCare",
                table: "coupon_usages",
                columns: new[] { "user_id", "coupon_id", "booking_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_transactions_booking_id",
                schema: "HomeCare",
                table: "transactions",
                column: "booking_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_transactions_service_id",
                schema: "HomeCare",
                table: "transactions",
                column: "service_id");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_transaction_uuid",
                schema: "HomeCare",
                table: "transactions",
                column: "transaction_uuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_transactions_user_id",
                schema: "HomeCare",
                table: "transactions",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "coupon_usages",
                schema: "HomeCare");

            migrationBuilder.DropTable(
                name: "support_tickets",
                schema: "HomeCare");

            migrationBuilder.DropTable(
                name: "transactions",
                schema: "HomeCare");

            migrationBuilder.DropTable(
                name: "bookings",
                schema: "HomeCare");

            migrationBuilder.DropColumn(
                name: "description",
                schema: "HomeCare",
                table: "services");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "HomeCare",
                table: "users",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "HomeCare",
                table: "user_otps",
                newName: "otp_id");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "HomeCare",
                table: "user_addresses",
                newName: "address_id");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "HomeCare",
                table: "sub_categories",
                newName: "sub_category_id");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "HomeCare",
                table: "services_images",
                newName: "service_image_id");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "HomeCare",
                table: "services",
                newName: "service_id");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "HomeCare",
                table: "service_types",
                newName: "service_id");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "HomeCare",
                table: "service_partners",
                newName: "sp_id");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "HomeCare",
                table: "service_partner_skills",
                newName: "spsk_id");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "HomeCare",
                table: "service_partner_services_offered",
                newName: "spso_id");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "HomeCare",
                table: "service_partner_languages",
                newName: "spl_id");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "HomeCare",
                table: "service_partner_experiences",
                newName: "spex_id");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "HomeCare",
                table: "service_partner_educations",
                newName: "spe_id");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "HomeCare",
                table: "service_partner_attachments",
                newName: "spa_id");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "HomeCare",
                table: "service_inclusions_exclusions",
                newName: "service_inclusion_exclusion_id");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "HomeCare",
                table: "offers",
                newName: "offer_id");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "HomeCare",
                table: "languages",
                newName: "language_id");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "HomeCare",
                table: "categories",
                newName: "category_id");

            migrationBuilder.AlterColumn<string>(
                name: "user_name",
                schema: "HomeCare",
                table: "users",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "user_email",
                schema: "HomeCare",
                table: "users",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "mobile_number",
                schema: "HomeCare",
                table: "users",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(15)",
                oldMaxLength: 15);

            migrationBuilder.AlterColumn<string>(
                name: "refresh_token_hash",
                schema: "HomeCare",
                table: "user_otps",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "otp_code",
                schema: "HomeCare",
                table: "user_otps",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<decimal>(
                name: "commission",
                schema: "HomeCare",
                table: "services",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(5,2)");
        }
    }
}
