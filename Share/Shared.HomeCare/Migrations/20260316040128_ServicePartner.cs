using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Shared.HomeCare.Migrations
{
    /// <inheritdoc />
    public partial class ServicePartner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_services_sub_categories_sub_category_id",
                schema: "HomeCare",
                table: "services");

            migrationBuilder.DropPrimaryKey(
                name: "PK_User_otps",
                schema: "HomeCare",
                table: "User_otps");

            migrationBuilder.DropIndex(
                name: "IX_services_sub_category_id_service_name",
                schema: "HomeCare",
                table: "services");

            migrationBuilder.RenameTable(
                name: "User_otps",
                schema: "HomeCare",
                newName: "user_otps",
                newSchema: "HomeCare");

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

            migrationBuilder.AlterColumn<string>(
                name: "service_name",
                schema: "HomeCare",
                table: "services",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "duration_minutes",
                schema: "HomeCare",
                table: "services",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<decimal>(
                name: "commission",
                schema: "HomeCare",
                table: "services",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(5,2)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_user_otps",
                schema: "HomeCare",
                table: "user_otps",
                column: "otp_id");

            migrationBuilder.CreateTable(
                name: "languages",
                schema: "HomeCare",
                columns: table => new
                {
                    language_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    language_name = table.Column<string>(type: "text", nullable: false),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_by = table.Column<int>(type: "integer", maxLength: 100, nullable: true),
                    modified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_languages", x => x.language_id);
                });

            migrationBuilder.CreateTable(
                name: "service_partner_attachments",
                schema: "HomeCare",
                columns: table => new
                {
                    spa_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    sp_id = table.Column<int>(type: "integer", nullable: false),
                    file_url = table.Column<string>(type: "text", nullable: false),
                    file_name = table.Column<string>(type: "text", nullable: false),
                    file_type = table.Column<string>(type: "text", nullable: true),
                    file_size_kb = table.Column<int>(type: "integer", nullable: true),
                    document_label = table.Column<string>(type: "text", nullable: true),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_by = table.Column<int>(type: "integer", maxLength: 100, nullable: true),
                    modified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_service_partner_attachments", x => x.spa_id);
                });

            migrationBuilder.CreateTable(
                name: "service_partner_educations",
                schema: "HomeCare",
                columns: table => new
                {
                    spe_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    sp_id = table.Column<int>(type: "integer", nullable: false),
                    school_college = table.Column<string>(type: "text", nullable: false),
                    passing_year = table.Column<int>(type: "integer", nullable: false),
                    marks = table.Column<decimal>(type: "numeric", nullable: true),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_by = table.Column<int>(type: "integer", maxLength: 100, nullable: true),
                    modified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_service_partner_educations", x => x.spe_id);
                });

            migrationBuilder.CreateTable(
                name: "service_partner_experiences",
                schema: "HomeCare",
                columns: table => new
                {
                    spex_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    sp_id = table.Column<int>(type: "integer", nullable: false),
                    company_name = table.Column<string>(type: "text", nullable: false),
                    role = table.Column<string>(type: "text", nullable: false),
                    from_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    to_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_by = table.Column<int>(type: "integer", maxLength: 100, nullable: true),
                    modified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_service_partner_experiences", x => x.spex_id);
                });

            migrationBuilder.CreateTable(
                name: "service_partner_languages",
                schema: "HomeCare",
                columns: table => new
                {
                    spl_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    sp_id = table.Column<int>(type: "integer", nullable: false),
                    language_id = table.Column<int>(type: "integer", nullable: false),
                    proficiency = table.Column<string>(type: "text", nullable: false),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_by = table.Column<int>(type: "integer", maxLength: 100, nullable: true),
                    modified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_service_partner_languages", x => x.spl_id);
                });

            migrationBuilder.CreateTable(
                name: "service_partner_services_offered",
                schema: "HomeCare",
                columns: table => new
                {
                    spso_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    sp_id = table.Column<int>(type: "integer", nullable: false),
                    sub_category_id = table.Column<int>(type: "integer", nullable: false),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_by = table.Column<int>(type: "integer", maxLength: 100, nullable: true),
                    modified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_service_partner_services_offered", x => x.spso_id);
                });

            migrationBuilder.CreateTable(
                name: "service_partner_skills",
                schema: "HomeCare",
                columns: table => new
                {
                    spsk_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    sp_id = table.Column<int>(type: "integer", nullable: false),
                    category_id = table.Column<int>(type: "integer", nullable: false),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_by = table.Column<int>(type: "integer", maxLength: 100, nullable: true),
                    modified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_service_partner_skills", x => x.spsk_id);
                });

            migrationBuilder.CreateTable(
                name: "service_partners",
                schema: "HomeCare",
                columns: table => new
                {
                    sp_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    full_name = table.Column<string>(type: "text", nullable: false),
                    date_of_birth = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    gender = table.Column<string>(type: "text", nullable: false),
                    mobile_number = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    applying_for_type_id = table.Column<int>(type: "integer", nullable: false),
                    permanent_address = table.Column<string>(type: "text", nullable: false),
                    residential_address = table.Column<string>(type: "text", nullable: false),
                    profile_image_url = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<string>(type: "text", nullable: false),
                    verification_status = table.Column<string>(type: "text", nullable: false),
                    verified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    verified_by = table.Column<int>(type: "integer", nullable: true),
                    rejection_reason = table.Column<string>(type: "text", nullable: true),
                    total_jobs_completed = table.Column<int>(type: "integer", nullable: false),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_by = table.Column<int>(type: "integer", maxLength: 100, nullable: true),
                    modified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_service_partners", x => x.sp_id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_services_sub_category_id",
                schema: "HomeCare",
                table: "services",
                column: "sub_category_id");

            migrationBuilder.AddForeignKey(
                name: "FK_services_sub_categories_sub_category_id",
                schema: "HomeCare",
                table: "services",
                column: "sub_category_id",
                principalSchema: "HomeCare",
                principalTable: "sub_categories",
                principalColumn: "sub_category_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_services_sub_categories_sub_category_id",
                schema: "HomeCare",
                table: "services");

            migrationBuilder.DropTable(
                name: "languages",
                schema: "HomeCare");

            migrationBuilder.DropTable(
                name: "service_partner_attachments",
                schema: "HomeCare");

            migrationBuilder.DropTable(
                name: "service_partner_educations",
                schema: "HomeCare");

            migrationBuilder.DropTable(
                name: "service_partner_experiences",
                schema: "HomeCare");

            migrationBuilder.DropTable(
                name: "service_partner_languages",
                schema: "HomeCare");

            migrationBuilder.DropTable(
                name: "service_partner_services_offered",
                schema: "HomeCare");

            migrationBuilder.DropTable(
                name: "service_partner_skills",
                schema: "HomeCare");

            migrationBuilder.DropTable(
                name: "service_partners",
                schema: "HomeCare");

            migrationBuilder.DropPrimaryKey(
                name: "PK_user_otps",
                schema: "HomeCare",
                table: "user_otps");

            migrationBuilder.DropIndex(
                name: "IX_services_sub_category_id",
                schema: "HomeCare",
                table: "services");

            migrationBuilder.RenameTable(
                name: "user_otps",
                schema: "HomeCare",
                newName: "User_otps",
                newSchema: "HomeCare");

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
                table: "User_otps",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "otp_code",
                schema: "HomeCare",
                table: "User_otps",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "service_name",
                schema: "HomeCare",
                table: "services",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "duration_minutes",
                schema: "HomeCare",
                table: "services",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<decimal>(
                name: "commission",
                schema: "HomeCare",
                table: "services",
                type: "numeric(5,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddPrimaryKey(
                name: "PK_User_otps",
                schema: "HomeCare",
                table: "User_otps",
                column: "otp_id");

            migrationBuilder.CreateIndex(
                name: "IX_services_sub_category_id_service_name",
                schema: "HomeCare",
                table: "services",
                columns: new[] { "sub_category_id", "service_name" },
                unique: true,
                filter: "\"is_deleted\" = false");

            migrationBuilder.AddForeignKey(
                name: "FK_services_sub_categories_sub_category_id",
                schema: "HomeCare",
                table: "services",
                column: "sub_category_id",
                principalSchema: "HomeCare",
                principalTable: "sub_categories",
                principalColumn: "sub_category_id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
