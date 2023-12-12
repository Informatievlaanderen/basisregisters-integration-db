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

        public AddressesLinkedToMultipleBuildingUnits() { }
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
                            FROM {Views.AddressesLinkedToMultipleBuildingUnits.Table} ");

            builder.HasIndex(x => x.AddressPersistentLocalId);
        }
    }
}
