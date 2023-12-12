using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace Basisregisters.IntegrationDb.Schema.Migrations
{
    public partial class Initial : Migration
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
                    NisCode = table.Column<int>(type: "integer", nullable: true),
                    PostalCode = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: true),
                    StreetNamePersistentLocalId = table.Column<int>(type: "integer", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    HouseNumber = table.Column<string>(type: "text", nullable: true),
                    BoxNumber = table.Column<string>(type: "text", nullable: true),
                    FullName = table.Column<string>(type: "text", nullable: true),
                    GeometryGml = table.Column<string>(type: "text", nullable: true),
                    Geometry = table.Column<Geometry>(type: "geometry", nullable: true, computedColumnSql: "ST_GeomFromGML(REPLACE(\"GeometryGml\",'https://www.opengis.net/def/crs/EPSG/0/', 'EPSG:')) ", stored: true),
                    PositionMethod = table.Column<string>(type: "text", nullable: true),
                    PositionSpecification = table.Column<string>(type: "text", nullable: true),
                    IsOfficiallyAssigned = table.Column<bool>(type: "boolean", nullable: true),
                    IsRemoved = table.Column<bool>(type: "boolean", nullable: false),
                    PuriId = table.Column<string>(type: "text", nullable: true),
                    Namespace = table.Column<string>(type: "text", nullable: true),
                    VersionString = table.Column<string>(type: "text", nullable: true),
                    VersionTimestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IdempotenceKey = table.Column<long>(type: "bigint", nullable: true)
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
                    Status = table.Column<string>(type: "text", nullable: true),
                    GeometryMethod = table.Column<string>(type: "text", nullable: true),
                    GeometryGml = table.Column<string>(type: "text", nullable: true),
                    Geometry = table.Column<Geometry>(type: "geometry", nullable: true, computedColumnSql: "ST_GeomFromGML(REPLACE(\"GeometryGml\",'https://www.opengis.net/def/crs/EPSG/0/', 'EPSG:')) ", stored: true),
                    IsRemoved = table.Column<bool>(type: "boolean", nullable: false),
                    PuriId = table.Column<string>(type: "text", nullable: true),
                    Namespace = table.Column<string>(type: "text", nullable: true),
                    VersionString = table.Column<string>(type: "text", nullable: true),
                    VersionTimestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IdempotenceKey = table.Column<long>(type: "bigint", nullable: true)
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
                    BuildingPersistentLocalId = table.Column<int>(type: "integer", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    Function = table.Column<string>(type: "text", nullable: true),
                    GeometryMethod = table.Column<string>(type: "text", nullable: true),
                    GeometryGml = table.Column<string>(type: "text", nullable: true),
                    Addresses = table.Column<string>(type: "text", nullable: true),
                    Geometry = table.Column<Geometry>(type: "geometry", nullable: true, computedColumnSql: "ST_GeomFromGML(REPLACE(\"GeometryGml\",'https://www.opengis.net/def/crs/EPSG/0/', 'EPSG:')) ", stored: true),
                    HasDeviation = table.Column<bool>(type: "boolean", nullable: true),
                    IsRemoved = table.Column<bool>(type: "boolean", nullable: false),
                    PuriId = table.Column<string>(type: "text", nullable: true),
                    Namespace = table.Column<string>(type: "text", nullable: true),
                    VersionString = table.Column<string>(type: "text", nullable: true),
                    VersionTimestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IdempotenceKey = table.Column<long>(type: "bigint", nullable: true)
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
                    Status = table.Column<string>(type: "text", nullable: true),
                    OfficialLanguageDutch = table.Column<bool>(type: "boolean", nullable: true),
                    OfficialLanguageFrench = table.Column<bool>(type: "boolean", nullable: true),
                    OfficialLanguageGerman = table.Column<bool>(type: "boolean", nullable: true),
                    OfficialLanguageEnglish = table.Column<bool>(type: "boolean", nullable: true),
                    FacilityLanguageDutch = table.Column<bool>(type: "boolean", nullable: true),
                    FacilityLanguageFrench = table.Column<bool>(type: "boolean", nullable: true),
                    FacilityLanguageGerman = table.Column<bool>(type: "boolean", nullable: true),
                    FacilityLanguageEnglish = table.Column<bool>(type: "boolean", nullable: true),
                    NameDutch = table.Column<string>(type: "text", nullable: true),
                    NameFrench = table.Column<string>(type: "text", nullable: true),
                    NameGerman = table.Column<string>(type: "text", nullable: true),
                    NameEnglish = table.Column<string>(type: "text", nullable: true),
                    IsRemoved = table.Column<bool>(type: "boolean", nullable: false),
                    PuriId = table.Column<string>(type: "text", nullable: true),
                    Namespace = table.Column<string>(type: "text", nullable: true),
                    VersionString = table.Column<string>(type: "text", nullable: true),
                    VersionTimestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IdempotenceKey = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Municipalities", x => x.NisCode);
                });

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

            migrationBuilder.CreateTable(
                name: "Parcels",
                schema: "Integration",
                columns: table => new
                {
                    CaPaKey = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: true),
                    Addresses = table.Column<string>(type: "text", nullable: true),
                    IsRemoved = table.Column<bool>(type: "boolean", nullable: false),
                    PuriId = table.Column<string>(type: "text", nullable: true),
                    Namespace = table.Column<string>(type: "text", nullable: true),
                    VersionString = table.Column<string>(type: "text", nullable: true),
                    VersionTimestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IdempotenceKey = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parcels", x => x.CaPaKey);
                });

            migrationBuilder.CreateTable(
                name: "PostInfo",
                schema: "Integration",
                columns: table => new
                {
                    PostalCode = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false),
                    NisCode = table.Column<int>(type: "integer", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    PostalNameDutch = table.Column<string>(type: "text", nullable: true),
                    PostalNameFrench = table.Column<string>(type: "text", nullable: true),
                    PostalNameGerman = table.Column<string>(type: "text", nullable: true),
                    PostalNameEnglish = table.Column<string>(type: "text", nullable: true),
                    PuriId = table.Column<string>(type: "text", nullable: true),
                    Namespace = table.Column<string>(type: "text", nullable: true),
                    VersionString = table.Column<string>(type: "text", nullable: true),
                    VersionTimestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IdempotenceKey = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostInfo", x => x.PostalCode);
                });

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

            migrationBuilder.CreateTable(
                name: "StreetNames",
                schema: "Integration",
                columns: table => new
                {
                    PersistentLocalId = table.Column<int>(type: "integer", nullable: false),
                    NisCode = table.Column<int>(type: "integer", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    NameDutch = table.Column<string>(type: "text", nullable: true),
                    NameFrench = table.Column<string>(type: "text", nullable: true),
                    NameGerman = table.Column<string>(type: "text", nullable: true),
                    NameEnglish = table.Column<string>(type: "text", nullable: true),
                    HomonymAdditionDutch = table.Column<string>(type: "text", nullable: true),
                    HomonymAdditionFrench = table.Column<string>(type: "text", nullable: true),
                    HomonymAdditionGerman = table.Column<string>(type: "text", nullable: true),
                    HomonymAdditionEnglish = table.Column<string>(type: "text", nullable: true),
                    IsRemoved = table.Column<bool>(type: "boolean", nullable: false),
                    PuriId = table.Column<string>(type: "text", nullable: true),
                    Namespace = table.Column<string>(type: "text", nullable: true),
                    VersionString = table.Column<string>(type: "text", nullable: true),
                    VersionTimestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IdempotenceKey = table.Column<long>(type: "bigint", nullable: true)
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
                column: "Geometry")
                .Annotation("Npgsql:IndexMethod", "GIST");

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
                name: "IX_Addresses_PersistentLocalId",
                schema: "Integration",
                table: "Addresses",
                column: "PersistentLocalId");

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
                column: "Geometry")
                .Annotation("Npgsql:IndexMethod", "GIST");

            migrationBuilder.CreateIndex(
                name: "IX_Buildings_IsRemoved",
                schema: "Integration",
                table: "Buildings",
                column: "IsRemoved");

            migrationBuilder.CreateIndex(
                name: "IX_Buildings_PersistentLocalId",
                schema: "Integration",
                table: "Buildings",
                column: "PersistentLocalId");

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
                column: "Geometry")
                .Annotation("Npgsql:IndexMethod", "GIST");

            migrationBuilder.CreateIndex(
                name: "IX_BuildingUnits_IsRemoved",
                schema: "Integration",
                table: "BuildingUnits",
                column: "IsRemoved");

            migrationBuilder.CreateIndex(
                name: "IX_BuildingUnits_PersistentLocalId",
                schema: "Integration",
                table: "BuildingUnits",
                column: "PersistentLocalId");

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
                name: "IX_Municipalities_NisCode",
                schema: "Integration",
                table: "Municipalities",
                column: "NisCode");

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

            migrationBuilder.CreateIndex(
                name: "IX_Parcels_CaPaKey",
                schema: "Integration",
                table: "Parcels",
                column: "CaPaKey");

            migrationBuilder.CreateIndex(
                name: "IX_Parcels_IsRemoved",
                schema: "Integration",
                table: "Parcels",
                column: "IsRemoved");

            migrationBuilder.CreateIndex(
                name: "IX_Parcels_Status",
                schema: "Integration",
                table: "Parcels",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Parcels_VersionTimestamp",
                schema: "Integration",
                table: "Parcels",
                column: "VersionTimestamp");

            migrationBuilder.CreateIndex(
                name: "IX_PostInfo_NisCode",
                schema: "Integration",
                table: "PostInfo",
                column: "NisCode");

            migrationBuilder.CreateIndex(
                name: "IX_PostInfo_PostalCode",
                schema: "Integration",
                table: "PostInfo",
                column: "PostalCode");

            migrationBuilder.CreateIndex(
                name: "IX_PostInfo_PostalNameDutch",
                schema: "Integration",
                table: "PostInfo",
                column: "PostalNameDutch");

            migrationBuilder.CreateIndex(
                name: "IX_PostInfo_PostalNameEnglish",
                schema: "Integration",
                table: "PostInfo",
                column: "PostalNameEnglish");

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
                name: "IX_StreetNames_NameEnglish",
                schema: "Integration",
                table: "StreetNames",
                column: "NameEnglish");

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
                name: "IX_StreetNames_PersistentLocalId",
                schema: "Integration",
                table: "StreetNames",
                column: "PersistentLocalId");

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
                name: "MunicipalityGeometries",
                schema: "Integration");

            migrationBuilder.DropTable(
                name: "Parcels",
                schema: "Integration");

            migrationBuilder.DropTable(
                name: "PostInfo",
                schema: "Integration");

            migrationBuilder.DropTable(
                name: "RoadSegments",
                schema: "Integration");

            migrationBuilder.DropTable(
                name: "StreetNames",
                schema: "Integration");
        }
    }
}
