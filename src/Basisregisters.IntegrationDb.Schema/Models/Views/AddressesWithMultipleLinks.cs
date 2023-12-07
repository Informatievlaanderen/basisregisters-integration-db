namespace Basisregisters.IntegrationDb.Schema.Models.Views
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class AddressesWithMultipleLinks
    {
        public int AddressPersistentLocalId { get; set; }
        public string Status { get; set; }
        public bool IsRemoved { get; set; }
        public int LinkedBuildingUnitCount { get; set; }
        public int LinkedParcelCount { get; set; }
        public int NisCode { get; set; }
        public DateTime Timestamp { get; set; }

        public AddressesWithMultipleLinks() { }
    }

    public sealed class AddressesWithMultipleLinksConfiguration : IEntityTypeConfiguration<AddressesWithMultipleLinks>
    {
        public void Configure(EntityTypeBuilder<AddressesWithMultipleLinks> builder)
        {
            builder
                .ToView(nameof(AddressesWithMultipleLinks), IntegrationContext.Schema)
                .HasNoKey()
                .ToSqlQuery(@$"
                            SELECT
                                ""AddressPersistentLocalId"",
                                ""Status"",
                                ""IsRemoved"",
                                ""LinkedBuildingUnitCount"",
                                ""LinkedParcelCount"",
                                ""NisCode"",
                                ""CURRENT_TIMESTAMP AS ""Timestamp""
                            FROM ""{IntegrationContext.Schema}"".""{nameof(ViewQueries.VIEW_AddressesWithMultipleLinks)}"" ");

            builder.HasIndex(x => x.AddressPersistentLocalId);
            builder.HasIndex(x => x.NisCode);
        }
    }
}
