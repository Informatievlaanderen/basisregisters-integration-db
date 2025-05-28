using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Basisregisters.IntegrationDb.Reporting.SuspiciousCases.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "integration_suspicious_cases");

            migrationBuilder.CreateTable(
                name: "reporting_suspicious_cases_monthly",
                schema: "integration_suspicious_cases",
                columns: table => new
                {
                    nis_code = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    suspicious_case_type = table.Column<string>(type: "text", nullable: false),
                    month = table.Column<DateOnly>(type: "date", nullable: false),
                    open_cases = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    closed_cases = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reporting_suspicious_cases_monthly", x => new { x.nis_code, x.suspicious_case_type, x.month });
                });

            migrationBuilder.CreateTable(
                name: "reporting_suspicious_cases",
                schema: "integration_suspicious_cases",
                columns: table => new
                {
                    nis_code = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    object_id = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    suspicious_case_type = table.Column<string>(type: "text", nullable: false),
                    object_type = table.Column<string>(type: "text", nullable: false),
                    date_added = table.Column<DateOnly>(type: "date", nullable: false),
                    date_closed = table.Column<DateOnly>(type: "date", nullable: true),
                    status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reporting_suspicious_cases", x => new { x.nis_code, x.object_id, x.suspicious_case_type });
                });

            migrationBuilder.CreateIndex(
                name: "IX_reporting_suspicious_cases_nis_code",
                schema: "integration_suspicious_cases",
                table: "reporting_suspicious_cases",
                column: "nis_code");

            migrationBuilder.CreateIndex(
                name: "IX_reporting_suspicious_cases_object_id_object_type",
                schema: "integration_suspicious_cases",
                table: "reporting_suspicious_cases",
                columns: new[] { "object_id", "object_type" });

            migrationBuilder.CreateIndex(
                name: "IX_reporting_suspicious_cases_status",
                schema: "integration_suspicious_cases",
                table: "reporting_suspicious_cases",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_reporting_suspicious_cases_suspicious_case_type",
                schema: "integration_suspicious_cases",
                table: "reporting_suspicious_cases",
                column: "suspicious_case_type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "reporting_suspicious_cases_monthly",
                schema: "integration_suspicious_cases");

            migrationBuilder.DropTable(
                name: "reporting_suspicious_cases",
                schema: "integration_suspicious_cases");
        }
    }
}
