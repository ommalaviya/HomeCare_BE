using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Shared.HomeCare.Migrations
{
    /// <inheritdoc />
    public partial class CleanStateAfterRefactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "modified_by",
                schema: "HomeCare",
                table: "sub_categories",
                type: "integer",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "modified_by",
                schema: "HomeCare",
                table: "service_types",
                type: "integer",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "modified_by",
                schema: "HomeCare",
                table: "categories",
                type: "integer",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "admins",
                schema: "HomeCare",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    mobile_number = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    address = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    password_hash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    profile_image_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_super_admin = table.Column<bool>(type: "boolean", nullable: false),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_by = table.Column<int>(type: "integer", nullable: true),
                    modified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_admins", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "offers",
                schema: "HomeCare",
                columns: table => new
                {
                    offer_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    coupon_code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    coupon_description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    discount_percentage = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    applied_count = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_by = table.Column<int>(type: "integer", nullable: true),
                    modified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_offers", x => x.offer_id);
                });

            migrationBuilder.CreateTable(
                name: "services",
                schema: "HomeCare",
                columns: table => new
                {
                    service_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    service_name = table.Column<string>(type: "text", nullable: false),
                    sub_category_id = table.Column<int>(type: "integer", nullable: false),
                    duration_minutes = table.Column<string>(type: "text", nullable: false),
                    price = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    commission = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    is_available = table.Column<bool>(type: "boolean", nullable: false),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_by = table.Column<int>(type: "integer", nullable: true),
                    modified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_services", x => x.service_id);
                    table.ForeignKey(
                        name: "FK_services_sub_categories_sub_category_id",
                        column: x => x.sub_category_id,
                        principalSchema: "HomeCare",
                        principalTable: "sub_categories",
                        principalColumn: "sub_category_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "User_otps",
                schema: "HomeCare",
                columns: table => new
                {
                    otp_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    otp_email = table.Column<string>(type: "text", nullable: false),
                    otp_code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    otp_expiry_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    otp_is_used = table.Column<bool>(type: "boolean", nullable: false),
                    otp_created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    refresh_token_hash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    refresh_token_expiry_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_otps", x => x.otp_id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                schema: "HomeCare",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    user_email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    is_email_verified = table.Column<bool>(type: "boolean", nullable: false),
                    mobile_number = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_by = table.Column<int>(type: "integer", nullable: true),
                    modified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "admin_password_reset_tokens",
                schema: "HomeCare",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    admin_id = table.Column<int>(type: "integer", nullable: false),
                    token = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_used = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_admin_password_reset_tokens", x => x.id);
                    table.ForeignKey(
                        name: "FK_admin_password_reset_tokens_admins_admin_id",
                        column: x => x.admin_id,
                        principalSchema: "HomeCare",
                        principalTable: "admins",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "admin_refresh_tokens",
                schema: "HomeCare",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    admin_id = table.Column<int>(type: "integer", nullable: false),
                    token_hash = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_revoked = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    replaced_by_token_hash = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_admin_refresh_tokens", x => x.id);
                    table.ForeignKey(
                        name: "FK_admin_refresh_tokens_admins_admin_id",
                        column: x => x.admin_id,
                        principalSchema: "HomeCare",
                        principalTable: "admins",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "service_inclusions_exclusions",
                schema: "HomeCare",
                columns: table => new
                {
                    service_inclusion_exclusion_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    service_id = table.Column<int>(type: "integer", nullable: false),
                    item = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_by = table.Column<int>(type: "integer", nullable: true),
                    modified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_service_inclusions_exclusions", x => x.service_inclusion_exclusion_id);
                    table.ForeignKey(
                        name: "FK_service_inclusions_exclusions_services_service_id",
                        column: x => x.service_id,
                        principalSchema: "HomeCare",
                        principalTable: "services",
                        principalColumn: "service_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "services_images",
                schema: "HomeCare",
                columns: table => new
                {
                    service_image_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    service_id = table.Column<int>(type: "integer", nullable: false),
                    image_name = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_by = table.Column<int>(type: "integer", nullable: true),
                    modified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_services_images", x => x.service_image_id);
                    table.ForeignKey(
                        name: "FK_services_images_services_service_id",
                        column: x => x.service_id,
                        principalSchema: "HomeCare",
                        principalTable: "services",
                        principalColumn: "service_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_addresses",
                schema: "HomeCare",
                columns: table => new
                {
                    address_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    house_flat_number = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    landmark = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    full_address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    save_as = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    latitude = table.Column<decimal>(type: "numeric(10,7)", nullable: false),
                    longitude = table.Column<decimal>(type: "numeric(10,7)", nullable: false),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_by = table.Column<int>(type: "integer", nullable: true),
                    modified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_addresses", x => x.address_id);
                    table.ForeignKey(
                        name: "FK_user_addresses_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "HomeCare",
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_admin_password_reset_tokens_admin_id",
                schema: "HomeCare",
                table: "admin_password_reset_tokens",
                column: "admin_id");

            migrationBuilder.CreateIndex(
                name: "IX_admin_password_reset_tokens_token",
                schema: "HomeCare",
                table: "admin_password_reset_tokens",
                column: "token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_admin_refresh_tokens_admin_id",
                schema: "HomeCare",
                table: "admin_refresh_tokens",
                column: "admin_id");

            migrationBuilder.CreateIndex(
                name: "IX_admin_refresh_tokens_token_hash",
                schema: "HomeCare",
                table: "admin_refresh_tokens",
                column: "token_hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_admins_email",
                schema: "HomeCare",
                table: "admins",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_service_inclusions_exclusions_service_id",
                schema: "HomeCare",
                table: "service_inclusions_exclusions",
                column: "service_id");

            migrationBuilder.CreateIndex(
                name: "IX_services_sub_category_id_service_name",
                schema: "HomeCare",
                table: "services",
                columns: new[] { "sub_category_id", "service_name" },
                unique: true,
                filter: "\"is_deleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_services_images_service_id",
                schema: "HomeCare",
                table: "services_images",
                column: "service_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_addresses_user_id",
                schema: "HomeCare",
                table: "user_addresses",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "admin_password_reset_tokens",
                schema: "HomeCare");

            migrationBuilder.DropTable(
                name: "admin_refresh_tokens",
                schema: "HomeCare");

            migrationBuilder.DropTable(
                name: "offers",
                schema: "HomeCare");

            migrationBuilder.DropTable(
                name: "service_inclusions_exclusions",
                schema: "HomeCare");

            migrationBuilder.DropTable(
                name: "services_images",
                schema: "HomeCare");

            migrationBuilder.DropTable(
                name: "user_addresses",
                schema: "HomeCare");

            migrationBuilder.DropTable(
                name: "User_otps",
                schema: "HomeCare");

            migrationBuilder.DropTable(
                name: "admins",
                schema: "HomeCare");

            migrationBuilder.DropTable(
                name: "services",
                schema: "HomeCare");

            migrationBuilder.DropTable(
                name: "users",
                schema: "HomeCare");

            migrationBuilder.AlterColumn<string>(
                name: "modified_by",
                schema: "HomeCare",
                table: "sub_categories",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "modified_by",
                schema: "HomeCare",
                table: "service_types",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "modified_by",
                schema: "HomeCare",
                table: "categories",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }
    }
}
