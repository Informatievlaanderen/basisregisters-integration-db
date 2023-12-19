namespace Basisregisters.IntegrationDb.Schema.Models.Views.SuspiciousCases
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class ActiveBuildingUnitWithoutAddress
    {
        public int BuildingUnitPersistentLocalId { get; set; }
        public int NisCode { get; set; }
        public DateTimeOffset Timestamp { get; set; }

        public ActiveBuildingUnitWithoutAddress()
        { }
    }

    public sealed class ActiveBuildingUnitWithoutAddressConfiguration : IEntityTypeConfiguration<ActiveBuildingUnitWithoutAddress>
    {
        public void Configure(EntityTypeBuilder<ActiveBuildingUnitWithoutAddress> builder)
        {
            builder
                .ToView(nameof(ActiveBuildingUnitWithoutAddress), IntegrationContext.Schema)
                .HasNoKey()
                .ToSqlQuery(@$"SELECT
                                ""BuildingUnitPersistentLocalId"",
                                ""NisCode"",
                                ""Timestamp""
                                FROM  {ViewName}");
        }

        public const string ViewName = @$"""{IntegrationContext.Schema}"".""VIEW_{nameof(ActiveBuildingUnitWithoutAddress)}""";

        public const string Create = $@"
            CREATE MATERIALIZED VIEW IF NOT EXISTS {ViewName} AS
                SELECT
                    bu.""PersistentLocalId"" AS ""BuildingUnitPersistentLocalId"",
                    muni.""NisCode"" AS ""NisCode"",
                    CURRENT_TIMESTAMP AS ""Timestamp""
                FROM ""Integration"".""BuildingUnits"" AS bu
                LEFT OUTER JOIN ""Integration"".""VIEW_BuildingUnitAddressRelations"" AS rel
                    ON bu.""PersistentLocalId"" = rel.""BuildingUnitPersistentLocalId""
                INNER JOIN ""Integration"".""MunicipalityGeometries"" AS muni
                    ON ST_Within(bu.""Geometry"", muni.""Geometry"") IS TRUE
                WHERE
                    bu.""IsRemoved"" IS FALSE
                    AND bu.""Status"" IN ('gepland','gerealiseerd')
                    AND rel.""AddressPersistentLocalId"" IS NULL
                ORDER BY bu.""PersistentLocalId"";

            CREATE INDEX ""IX_BuildingUnitPersistentLocalId"" ON {ViewName} USING btree (""{nameof(ActiveBuildingUnitWithoutAddress.BuildingUnitPersistentLocalId)}"");
            CREATE INDEX ""IX_NisCode"" ON {ViewName} USING btree (""{nameof(ActiveBuildingUnitWithoutAddress.NisCode)}"");
            ";
    }
}
