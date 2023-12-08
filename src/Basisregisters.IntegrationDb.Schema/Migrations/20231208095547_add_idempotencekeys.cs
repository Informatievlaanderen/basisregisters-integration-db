using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Basisregisters.IntegrationDb.Schema.Migrations
{
    public partial class add_idempotencekeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "IdempotenceKey",
                schema: "Integration",
                table: "StreetNames",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "IdempotenceKey",
                schema: "Integration",
                table: "PostInfo",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "IdempotenceKey",
                schema: "Integration",
                table: "Parcels",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "IdempotenceKey",
                schema: "Integration",
                table: "Municipalities",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "IdempotenceKey",
                schema: "Integration",
                table: "BuildingUnits",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "IdempotenceKey",
                schema: "Integration",
                table: "Buildings",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "IdempotenceKey",
                schema: "Integration",
                table: "Addresses",
                type: "bigint",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdempotenceKey",
                schema: "Integration",
                table: "StreetNames");

            migrationBuilder.DropColumn(
                name: "IdempotenceKey",
                schema: "Integration",
                table: "PostInfo");

            migrationBuilder.DropColumn(
                name: "IdempotenceKey",
                schema: "Integration",
                table: "Parcels");

            migrationBuilder.DropColumn(
                name: "IdempotenceKey",
                schema: "Integration",
                table: "Municipalities");

            migrationBuilder.DropColumn(
                name: "IdempotenceKey",
                schema: "Integration",
                table: "BuildingUnits");

            migrationBuilder.DropColumn(
                name: "IdempotenceKey",
                schema: "Integration",
                table: "Buildings");

            migrationBuilder.DropColumn(
                name: "IdempotenceKey",
                schema: "Integration",
                table: "Addresses");
        }
    }
}
