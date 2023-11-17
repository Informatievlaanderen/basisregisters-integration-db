using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace Basisregisters.Integration.Db.Schema.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Integration");

            migrationBuilder.CreateTable(
                name: "Addresses",
                schema: "Integration",
                columns: table => new
                {
                    PersistentLocalId = table.Column<int>(type: "integer", nullable: false),
                    NisCode = table.Column<int>(type: "integer", nullable: false),
                    PostalCode = table.Column<string>(type: "text", nullable: true),
                    StreetNamePersistentLocalId = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    HouseNumber = table.Column<string>(type: "text", nullable: false),
                    BoxNumber = table.Column<string>(type: "text", nullable: true),
                    FullNameDutch = table.Column<string>(type: "text", nullable: true),
                    FullNameFrench = table.Column<string>(type: "text", nullable: true),
                    FullNameGerman = table.Column<string>(type: "text", nullable: true),
                    FullNameEnglish = table.Column<string>(type: "text", nullable: true),
                    GeometryGml = table.Column<string>(type: "text", nullable: false),
                    Geometry = table.Column<Geometry>(type: "geometry", nullable: false, computedColumnSql: "ST_GeomFromGML(REPLACE(\"GeometryGml\",'https://www.opengis.net/def/crs/EPSG/0/', 'EPSG:')) ", stored: true),
                    PositionMethod = table.Column<string>(type: "text", nullable: false),
                    PositionSpecification = table.Column<string>(type: "text", nullable: false),
                    IsOfficiallyAssigned = table.Column<bool>(type: "boolean", nullable: false),
                    PuriId = table.Column<string>(type: "text", nullable: false),
                    Namespace = table.Column<string>(type: "text", nullable: false),
                    VerionString = table.Column<string>(type: "text", nullable: false),
                    VersionTimestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsRemoved = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.PersistentLocalId);
                });

            migrationBuilder.CreateTable(
                name: "Buildings",
                schema: "Integration",
                columns: table => new
                {
                    PersistentLocalId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    GeometryMethod = table.Column<string>(type: "text", nullable: false),
                    GeometryGml = table.Column<string>(type: "text", nullable: false),
                    Geometry = table.Column<Geometry>(type: "geometry", nullable: false, computedColumnSql: "ST_GeomFromGML(REPLACE(\"GeometryGml\",'https://www.opengis.net/def/crs/EPSG/0/', 'EPSG:')) ", stored: true),
                    PuriId = table.Column<string>(type: "text", nullable: false),
                    Namespace = table.Column<string>(type: "text", nullable: false),
                    VerionString = table.Column<string>(type: "text", nullable: false),
                    VersionTimestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsRemoved = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Buildings", x => x.PersistentLocalId);
                });

            migrationBuilder.CreateTable(
                name: "BuildingUnits",
                schema: "Integration",
                columns: table => new
                {
                    PersistentLocalId = table.Column<int>(type: "integer", nullable: false),
                    BuildingPersistentLocalId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Function = table.Column<string>(type: "text", nullable: false),
                    GeometryMethod = table.Column<string>(type: "text", nullable: false),
                    GeometryGml = table.Column<string>(type: "text", nullable: false),
                    Geometry = table.Column<Geometry>(type: "geometry", nullable: false, computedColumnSql: "ST_GeomFromGML(REPLACE(\"GeometryGml\",'https://www.opengis.net/def/crs/EPSG/0/', 'EPSG:')) ", stored: true),
                    HasDeviation = table.Column<bool>(type: "boolean", nullable: false),
                    PuriId = table.Column<string>(type: "text", nullable: false),
                    Namespace = table.Column<string>(type: "text", nullable: false),
                    VerionString = table.Column<string>(type: "text", nullable: false),
                    VersionTimestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsRemoved = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuildingUnits", x => x.PersistentLocalId);
                });

            migrationBuilder.CreateTable(
                name: "Municipalities",
                schema: "Integration",
                columns: table => new
                {
                    NisCode = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    OfficialLanguageDutch = table.Column<bool>(type: "boolean", nullable: false),
                    OfficialLanguageFrench = table.Column<bool>(type: "boolean", nullable: false),
                    OfficialLanguageGerman = table.Column<bool>(type: "boolean", nullable: false),
                    OfficialLanguageEnglish = table.Column<bool>(type: "boolean", nullable: false),
                    FacilityLanguageDutch = table.Column<bool>(type: "boolean", nullable: false),
                    FacilityLanguageFrench = table.Column<bool>(type: "boolean", nullable: false),
                    FacilityLanguageGerman = table.Column<bool>(type: "boolean", nullable: false),
                    FacilityLanguageEnglish = table.Column<bool>(type: "boolean", nullable: false),
                    NameDutch = table.Column<string>(type: "text", nullable: true),
                    NameFrench = table.Column<string>(type: "text", nullable: true),
                    NameGerman = table.Column<string>(type: "text", nullable: true),
                    NameEnglish = table.Column<string>(type: "text", nullable: true),
                    PuriId = table.Column<string>(type: "text", nullable: false),
                    Namespace = table.Column<string>(type: "text", nullable: false),
                    VerionString = table.Column<string>(type: "text", nullable: false),
                    VersionTimestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsRemoved = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Municipalities", x => x.NisCode);
                });

            migrationBuilder.CreateTable(
                name: "PostInfo",
                schema: "Integration",
                columns: table => new
                {
                    PostalCode = table.Column<string>(type: "text", nullable: false),
                    NisCode = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    PostalNameDutch = table.Column<string>(type: "text", nullable: true),
                    PostalNameFrench = table.Column<string>(type: "text", nullable: true),
                    PostalNameGerman = table.Column<string>(type: "text", nullable: true),
                    PuriId = table.Column<string>(type: "text", nullable: false),
                    Namespace = table.Column<string>(type: "text", nullable: false),
                    VerionString = table.Column<string>(type: "text", nullable: false),
                    VersionTimestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostInfo", x => x.PostalCode);
                });

            migrationBuilder.CreateTable(
                name: "StreetNames",
                schema: "Integration",
                columns: table => new
                {
                    PersistentLocalId = table.Column<int>(type: "integer", nullable: false),
                    NisCode = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    NameDutch = table.Column<string>(type: "text", nullable: false),
                    NameFrench = table.Column<string>(type: "text", nullable: false),
                    NameGerman = table.Column<string>(type: "text", nullable: false),
                    HomonymAdditionDutch = table.Column<string>(type: "text", nullable: false),
                    HomonymAdditionFrench = table.Column<string>(type: "text", nullable: false),
                    HomonymAdditionGerman = table.Column<string>(type: "text", nullable: false),
                    PuriId = table.Column<string>(type: "text", nullable: false),
                    Namespace = table.Column<string>(type: "text", nullable: false),
                    VerionString = table.Column<string>(type: "text", nullable: false),
                    VersionTimestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsRemoved = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StreetNames", x => x.PersistentLocalId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_BoxNumber",
                schema: "Integration",
                table: "Addresses",
                column: "BoxNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_Geometry",
                schema: "Integration",
                table: "Addresses",
                column: "Geometry");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_HouseNumber",
                schema: "Integration",
                table: "Addresses",
                column: "HouseNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_IsRemoved",
                schema: "Integration",
                table: "Addresses",
                column: "IsRemoved");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_NisCode",
                schema: "Integration",
                table: "Addresses",
                column: "NisCode");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_PostalCode",
                schema: "Integration",
                table: "Addresses",
                column: "PostalCode");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_Status",
                schema: "Integration",
                table: "Addresses",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_StreetNamePersistentLocalId",
                schema: "Integration",
                table: "Addresses",
                column: "StreetNamePersistentLocalId");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_VersionTimestamp",
                schema: "Integration",
                table: "Addresses",
                column: "VersionTimestamp");

            migrationBuilder.CreateIndex(
                name: "IX_Buildings_Geometry",
                schema: "Integration",
                table: "Buildings",
                column: "Geometry");

            migrationBuilder.CreateIndex(
                name: "IX_Buildings_IsRemoved",
                schema: "Integration",
                table: "Buildings",
                column: "IsRemoved");

            migrationBuilder.CreateIndex(
                name: "IX_Buildings_Status",
                schema: "Integration",
                table: "Buildings",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Buildings_VersionTimestamp",
                schema: "Integration",
                table: "Buildings",
                column: "VersionTimestamp");

            migrationBuilder.CreateIndex(
                name: "IX_BuildingUnits_Geometry",
                schema: "Integration",
                table: "BuildingUnits",
                column: "Geometry");

            migrationBuilder.CreateIndex(
                name: "IX_BuildingUnits_IsRemoved",
                schema: "Integration",
                table: "BuildingUnits",
                column: "IsRemoved");

            migrationBuilder.CreateIndex(
                name: "IX_BuildingUnits_Status",
                schema: "Integration",
                table: "BuildingUnits",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_BuildingUnits_VersionTimestamp",
                schema: "Integration",
                table: "BuildingUnits",
                column: "VersionTimestamp");

            migrationBuilder.CreateIndex(
                name: "IX_Municipalities_IsRemoved",
                schema: "Integration",
                table: "Municipalities",
                column: "IsRemoved");

            migrationBuilder.CreateIndex(
                name: "IX_Municipalities_NameDutch",
                schema: "Integration",
                table: "Municipalities",
                column: "NameDutch");

            migrationBuilder.CreateIndex(
                name: "IX_Municipalities_NameEnglish",
                schema: "Integration",
                table: "Municipalities",
                column: "NameEnglish");

            migrationBuilder.CreateIndex(
                name: "IX_Municipalities_NameFrench",
                schema: "Integration",
                table: "Municipalities",
                column: "NameFrench");

            migrationBuilder.CreateIndex(
                name: "IX_Municipalities_NameGerman",
                schema: "Integration",
                table: "Municipalities",
                column: "NameGerman");

            migrationBuilder.CreateIndex(
                name: "IX_Municipalities_Status",
                schema: "Integration",
                table: "Municipalities",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Municipalities_VersionTimestamp",
                schema: "Integration",
                table: "Municipalities",
                column: "VersionTimestamp");

            migrationBuilder.CreateIndex(
                name: "IX_PostInfo_NisCode",
                schema: "Integration",
                table: "PostInfo",
                column: "NisCode");

            migrationBuilder.CreateIndex(
                name: "IX_PostInfo_PostalNameDutch",
                schema: "Integration",
                table: "PostInfo",
                column: "PostalNameDutch");

            migrationBuilder.CreateIndex(
                name: "IX_PostInfo_PostalNameFrench",
                schema: "Integration",
                table: "PostInfo",
                column: "PostalNameFrench");

            migrationBuilder.CreateIndex(
                name: "IX_PostInfo_PostalNameGerman",
                schema: "Integration",
                table: "PostInfo",
                column: "PostalNameGerman");

            migrationBuilder.CreateIndex(
                name: "IX_PostInfo_Status",
                schema: "Integration",
                table: "PostInfo",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_PostInfo_VersionTimestamp",
                schema: "Integration",
                table: "PostInfo",
                column: "VersionTimestamp");

            migrationBuilder.CreateIndex(
                name: "IX_StreetNames_IsRemoved",
                schema: "Integration",
                table: "StreetNames",
                column: "IsRemoved");

            migrationBuilder.CreateIndex(
                name: "IX_StreetNames_NameDutch",
                schema: "Integration",
                table: "StreetNames",
                column: "NameDutch");

            migrationBuilder.CreateIndex(
                name: "IX_StreetNames_NameFrench",
                schema: "Integration",
                table: "StreetNames",
                column: "NameFrench");

            migrationBuilder.CreateIndex(
                name: "IX_StreetNames_NameGerman",
                schema: "Integration",
                table: "StreetNames",
                column: "NameGerman");

            migrationBuilder.CreateIndex(
                name: "IX_StreetNames_NisCode",
                schema: "Integration",
                table: "StreetNames",
                column: "NisCode");

            migrationBuilder.CreateIndex(
                name: "IX_StreetNames_Status",
                schema: "Integration",
                table: "StreetNames",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_StreetNames_VersionTimestamp",
                schema: "Integration",
                table: "StreetNames",
                column: "VersionTimestamp");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Addresses",
                schema: "Integration");

            migrationBuilder.DropTable(
                name: "Buildings",
                schema: "Integration");

            migrationBuilder.DropTable(
                name: "BuildingUnits",
                schema: "Integration");

            migrationBuilder.DropTable(
                name: "Municipalities",
                schema: "Integration");

            migrationBuilder.DropTable(
                name: "PostInfo",
                schema: "Integration");

            migrationBuilder.DropTable(
                name: "StreetNames",
                schema: "Integration");
        }
    }
}
