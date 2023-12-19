namespace Basisregisters.IntegrationDb.Schema.Models.Views
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class BuildingUnitAddressRelations
    {
        public int BuildingUnitPersistentLocalId { get; set; }
        public string AddressPersistentLocalId { get; set; }
        public DateTimeOffset Timestamp { get; set; }

        public BuildingUnitAddressRelations() { }
    }

    public sealed class BuildingUnitAddressRelationConfiguration : IEntityTypeConfiguration<BuildingUnitAddressRelations>
    {
        public void Configure(EntityTypeBuilder<BuildingUnitAddressRelations> builder)
        {
            builder
                .ToView(nameof(BuildingUnitAddressRelations), IntegrationContext.Schema)
                .HasNoKey()
                .ToSqlQuery(@$"
                            SELECT
                                ""BuildingUnitPersistentLocalId"",
                                ""AddressPersistentLocalId"",
                                 ""Timestamp""
                            FROM  {ViewName} ");
        }

        public const string ViewName = @$"""{IntegrationContext.Schema}"".""VIEW_{nameof(BuildingUnitAddressRelations)}""";

        public const string Create = $@"
            CREATE MATERIALIZED VIEW IF NOT EXISTS {ViewName} AS
            SELECT
                bu.""PersistentLocalId"" AS ""BuildingUnitPersistentLocalId"",
                unnested_address_id AS ""AddressPersistentLocalId"",
                CURRENT_TIMESTAMP AS ""Timestamp""
            FROM
                ""Integration"".""BuildingUnits"" bu
            WHERE
                bu.""IsRemoved"" = false
            CROSS JOIN
                unnest(string_to_array(bu.""Addresses"", ', ')::int[]) AS unnested_address_id;

            CREATE INDEX ""IX_Address_PersistentLocalId"" ON ""Integration"".""{ViewName}""(""{nameof(BuildingUnitAddressRelations.AddressPersistentLocalId)}"")
            CREATE INDEX ""IX_BuildingUnit_PersistentLocalId"" ON ""Integration"".""{ViewName}""(""{nameof(BuildingUnitAddressRelations.BuildingUnitPersistentLocalId)}"")
            ";
    }
}
