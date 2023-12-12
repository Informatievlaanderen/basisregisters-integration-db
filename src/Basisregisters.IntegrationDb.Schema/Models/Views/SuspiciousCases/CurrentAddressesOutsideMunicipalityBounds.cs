namespace Basisregisters.IntegrationDb.Schema.Models.Views.SuspiciousCases
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class CurrentAddressesOutsideMunicipalityBounds
    {
        public int AddressPersistentLocalId { get; set; }
        public int NisCode { get; set; }
        public DateTimeOffset Timestamp { get; set; }

        public CurrentAddressesOutsideMunicipalityBounds() { }
    }

    public sealed class CurrentAddressesOutsideMunicipalityBoundsConfiguration : IEntityTypeConfiguration<CurrentAddressesOutsideMunicipalityBounds>
    {
        public void Configure(EntityTypeBuilder<CurrentAddressesOutsideMunicipalityBounds> builder)
        {
            builder
                .ToView(nameof(CurrentAddressesOutsideMunicipalityBounds), IntegrationContext.Schema)
                .HasNoKey()
                .ToSqlQuery(@$"
                            SELECT
                                ""AddressPersistentLocalId"",
                                ""NisCode"",
                                ""Timestamp""
                            FROM  {Views.CurrentAddressesOutsideMunicipalityBounds.Table} ");

            builder.HasIndex(x => x.NisCode);
        }
    }
}
