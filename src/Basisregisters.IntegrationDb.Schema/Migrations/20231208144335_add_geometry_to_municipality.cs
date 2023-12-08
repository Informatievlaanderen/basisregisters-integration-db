using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace Basisregisters.IntegrationDb.Schema.Migrations
{
    public partial class add_geometry_to_municipality : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MunicipalityGeometries",
                schema: "Integration",
                columns: table => new
                {
                    NisCode = table.Column<int>(type: "integer", nullable: false),
                    Geometry = table.Column<Geometry>(type: "geometry", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MunicipalityGeometries", x => x.NisCode);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MunicipalityGeometries_Geometry",
                schema: "Integration",
                table: "MunicipalityGeometries",
                column: "Geometry")
                .Annotation("Npgsql:IndexMethod", "GIST");

            migrationBuilder.CreateIndex(
                name: "IX_MunicipalityGeometries_NisCode",
                schema: "Integration",
                table: "MunicipalityGeometries",
                column: "NisCode");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MunicipalityGeometries",
                schema: "Integration");
        }
    }
}
