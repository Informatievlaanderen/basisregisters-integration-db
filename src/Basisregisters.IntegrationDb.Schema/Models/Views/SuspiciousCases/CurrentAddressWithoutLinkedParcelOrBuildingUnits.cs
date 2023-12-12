namespace Basisregisters.IntegrationDb.Schema.Models.Views.SuspiciousCases
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class CurrentAddressWithoutLinkedParcelOrBuildingUnits
    {
        public int AddressPersistentLocalId { get; set; }
        public int NisCode { get; set; }
        public DateTimeOffset Timestamp { get; set; }

        public CurrentAddressWithoutLinkedParcelOrBuildingUnits() { }
    }

    public sealed class ActiveAddressWithoutLinkedParcelOrBuildingUnitsConfiguration : IEntityTypeConfiguration<CurrentAddressWithoutLinkedParcelOrBuildingUnits>
    {
        public void Configure(EntityTypeBuilder<CurrentAddressWithoutLinkedParcelOrBuildingUnits> builder)
        {
            builder
                .ToView(nameof(CurrentAddressWithoutLinkedParcelOrBuildingUnits), IntegrationContext.Schema)
                .HasNoKey()
                .ToSqlQuery(@$"SELECT
                                ""AddressPersistentLocalId"",
                                ""NisCode"",
                                ""Timestamp""
                                FROM  {Views.CurrentAddressWithoutLinkedParcelOrBuildingUnit.Table} ");

            builder.HasIndex(x => x.AddressPersistentLocalId);
            builder.HasIndex(x => x.NisCode);
        }
    }
}
