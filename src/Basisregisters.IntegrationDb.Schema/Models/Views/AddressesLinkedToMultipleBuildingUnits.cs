namespace Basisregisters.IntegrationDb.Schema.Models.Views
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class AddressesLinkedToMultipleBuildingUnits
    {
        public int AddressPersistentLocalId { get; set; }
        public int LinkedBuildingUnitCount { get; set; }
        public int NisCode { get; set; }
        public DateTime Timestamp { get; set; }
        public string Status { get; set; }

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
                            FROM ""{IntegrationContext.Schema}"".""{nameof(ViewQueries.VIEW_AddressesLinkedToMultipleBuildingUnits)}"" ");

            builder.HasIndex(x => x.AddressPersistentLocalId);
            builder.HasIndex(x => x.Status);
        }
    }
}
