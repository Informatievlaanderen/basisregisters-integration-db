using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace Basisregisters.IntegrationDb.Schema.Migrations
{
    public partial class add_table_roadsegment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RoadSegments",
                schema: "Integration",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: true),
                    AccessRestrictionDutchName = table.Column<string>(type: "text", nullable: true),
                    AccessRestrictionId = table.Column<int>(type: "integer", nullable: true),
                    BeginRoadNodeId = table.Column<string>(type: "text", nullable: true),
                    CategoryDutchName = table.Column<string>(type: "text", nullable: true),
                    CategoryId = table.Column<string>(type: "text", nullable: true),
                    EndRoadNodeId = table.Column<int>(type: "integer", nullable: true),
                    GeometryAsHex = table.Column<Geometry>(type: "geometry", nullable: true, computedColumnSql: "ST_AsHEXEWKB(ST_GeomFromText(\"GeometryAsWkt\"))", stored: true),
                    GeometryAsWkt = table.Column<string>(type: "text", nullable: true),
                    GeometrySrid = table.Column<int>(type: "integer", nullable: true),
                    GeometryVersion = table.Column<int>(type: "integer", nullable: true),
                    LeftSideMunicipalityId = table.Column<string>(type: "text", nullable: true),
                    LeftSideMunicipalityNisCode = table.Column<string>(type: "text", nullable: true),
                    LeftSideStreetName = table.Column<string>(type: "text", nullable: true),
                    LeftSideStreetNameId = table.Column<int>(type: "integer", nullable: true),
                    MaintainerId = table.Column<string>(type: "text", nullable: true),
                    MaintainerName = table.Column<string>(type: "text", nullable: true),
                    MethodDutchName = table.Column<string>(type: "text", nullable: true),
                    MethodId = table.Column<int>(type: "integer", nullable: true),
                    MorphologyDutchName = table.Column<string>(type: "text", nullable: true),
                    MorphologyId = table.Column<int>(type: "integer", nullable: true),
                    RecordingDate = table.Column<string>(type: "text", nullable: true),
                    RightSideMunicipalityId = table.Column<string>(type: "text", nullable: true),
                    RightSideMunicipalityNisCode = table.Column<string>(type: "text", nullable: true),
                    RightSideStreetName = table.Column<string>(type: "text", nullable: true),
                    RightSideStreetNameId = table.Column<int>(type: "integer", nullable: true),
                    RoadSegmentVersion = table.Column<int>(type: "integer", nullable: true),
                    StatusDutchName = table.Column<string>(type: "text", nullable: true),
                    StatusId = table.Column<int>(type: "integer", nullable: true),
                    StreetNameCachePosition = table.Column<int>(type: "integer", nullable: true),
                    TransactionId = table.Column<int>(type: "integer", nullable: true),
                    Timestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Organization = table.Column<string>(type: "text", nullable: true),
                    LastChangedTimestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Removed = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoadSegments", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoadSegments_GeometryAsHex",
                schema: "Integration",
                table: "RoadSegments",
                column: "GeometryAsHex")
                .Annotation("Npgsql:IndexMethod", "GIST");

            migrationBuilder.CreateIndex(
                name: "IX_RoadSegments_MorphologyId",
                schema: "Integration",
                table: "RoadSegments",
                column: "MorphologyId");

            migrationBuilder.CreateIndex(
                name: "IX_RoadSegments_StreetNameCachePosition",
                schema: "Integration",
                table: "RoadSegments",
                column: "StreetNameCachePosition");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoadSegments",
                schema: "Integration");
        }
    }
}
