namespace Basisregisters.IntegrationDb.Schema.Models.Views.SuspiciousCases
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class BuildingUnitLinkedToMultipleAddresses
    {
        public int BuildingUnitPersistentLocalId { get; set; }
        public int NisCode { get; set; }
        public DateTimeOffset Timestamp { get; set; }

        public BuildingUnitLinkedToMultipleAddresses()
        { }
    }

    public sealed class BuildingUnitLinkedToMultipleAddressesConfiguration : IEntityTypeConfiguration<BuildingUnitLinkedToMultipleAddresses>
    {
        public void Configure(EntityTypeBuilder<BuildingUnitLinkedToMultipleAddresses> builder)
        {
            builder
                .ToView(nameof(BuildingUnitLinkedToMultipleAddresses), SuspiciousCasesContext.Schema)
                .HasNoKey()
                .ToSqlQuery(@$"SELECT
                                ""BuildingUnitPersistentLocalId"",
                                ""NisCode"",
                                ""Timestamp""
                                FROM  {ViewName}");
        }

        public const string ViewName = @$"""{SuspiciousCasesContext.Schema}"".""VIEW_{nameof(BuildingUnitLinkedToMultipleAddresses)}""";

        public const string Create = $@"
            SELECT
                bu.""PersistentLocalId"" AS ""BuildingUnitPersistentLocalId"",
                MIN(muni.""NisCode"") AS ""NisCode"",
                CURRENT_TIMESTAMP AS ""Timestamp""
            FROM ""Integration"".""BuildingUnits"" AS bu
            INNER JOIN ""Integration"".""VIEW_BuildingUnitAddressRelations"" AS rel ON bu.""PersistentLocalId"" = rel.""BuildingUnitPersistentLocalId""
            INNER JOIN ""Integration"".""Addresses"" AS ad ON rel.""AddressPersistentLocalId"" = ad.""PersistentLocalId""
            INNER JOIN ""Integration"".""MunicipalityGeometries"" AS muni ON ST_Within(bu.""Geometry"", muni.""Geometry"") IS TRUE
            WHERE
                bu.""IsRemoved"" IS FALSE
                AND bu.""Status"" IN ('gepland','gerealiseerd')
            GROUP BY bu.""PersistentLocalId""
            HAVING COUNT(*) > 1;

            CREATE INDEX ""IX_BuildingUnitPersistentLocalId"" ON {ViewName} USING btree (""{nameof(BuildingUnitLinkedToMultipleAddresses.BuildingUnitPersistentLocalId)}"");
            CREATE INDEX ""IX_NisCode"" ON {ViewName} USING btree (""{nameof(BuildingUnitLinkedToMultipleAddresses.NisCode)}"");
            ";
    }
}
