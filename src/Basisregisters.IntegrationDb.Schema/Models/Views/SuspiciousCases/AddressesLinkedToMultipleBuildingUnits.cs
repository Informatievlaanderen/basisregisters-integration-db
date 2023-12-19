namespace Basisregisters.IntegrationDb.Schema.Models.Views.SuspiciousCases
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class AddressesLinkedToMultipleBuildingUnits
    {
        public int AddressPersistentLocalId { get; set; }
        public int LinkedBuildingUnitCount { get; set; }
        public int NisCode { get; set; }
        public DateTimeOffset Timestamp { get; set; }

        public AddressesLinkedToMultipleBuildingUnits()
        { }
    }

    public sealed class AddressesLinkedToMultipleBuildingUnitsConfiguration : IEntityTypeConfiguration<AddressesLinkedToMultipleBuildingUnits>
    {
        public void Configure(EntityTypeBuilder<AddressesLinkedToMultipleBuildingUnits> builder)
        {
            builder
                .ToView(nameof(AddressesLinkedToMultipleBuildingUnits), IntegrationContext.Schema)
                .HasNoKey()
                .ToSqlQuery(@$"
                            SELECT
                                ""AddressPersistentLocalId"",
                                ""LinkedBuildingUnitCount"",
                                ""NisCode"",
                                ""Timestamp""
                            FROM {ViewName} ");
        }


        public const string ViewName = @$"""{IntegrationContext.Schema}"".""VIEW_{nameof(AddressesLinkedToMultipleBuildingUnits)}""";

        public const string Create = $@"
            CREATE MATERIALIZED VIEW IF NOT EXISTS {ViewName} AS
            SELECT
                a.""PersistentLocalId"" AS ""AddressPersistentLocalId"",
                COUNT(*) AS ""LinkedBuildingUnitCount"",
                a.""NisCode"",
                CURRENT_TIMESTAMP AS ""Timestamp""

            FROM {BuildingUnitAddressRelationConfiguration.ViewName} relation
            JOIN ""Integration"".""Addresses"" a ON a.""PersistentLocalId"" = relation.""AddressPersistentLocalId""
            WHERE
                a.""IsRemoved"" = false
            GROUP BY
                a.""PersistentLocalId""
            HAVING
                COUNT(*) > 1;

            CREATE INDEX ""IX_{nameof(AddressesLinkedToMultipleBuildingUnits)}_AddressPersistentLocalId"" ON {ViewName} USING btree (""{nameof(AddressesLinkedToMultipleBuildingUnits.AddressPersistentLocalId)}"");
        ";
    }
}
