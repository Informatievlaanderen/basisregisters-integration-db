namespace Basisregisters.IntegrationDb.Schema.Models.Views.SuspiciousCases
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class ProposedAddressWithoutLinkedParcelOrBuildingUnits
    {
        public int AddressPersistentLocalId { get; set; }
        public int NisCode { get; set; }
        public DateTimeOffset Timestamp { get; set; }

        public ProposedAddressWithoutLinkedParcelOrBuildingUnits() { }
    }

    public sealed class ProposedAddressWithoutLinkedParcelOrBuildingUnitsConfiguration : IEntityTypeConfiguration<ProposedAddressWithoutLinkedParcelOrBuildingUnits>
    {
        public void Configure(EntityTypeBuilder<ProposedAddressWithoutLinkedParcelOrBuildingUnits> builder)
        {
            builder
                .ToView(nameof(ProposedAddressWithoutLinkedParcelOrBuildingUnits), IntegrationContext.Schema)
                .HasNoKey()
                .ToSqlQuery(@$"SELECT
                                ""AddressPersistentLocalId"",
                                ""NisCode"",
                                ""Timestamp""
                                FROM  {Views.ProposedAddressWithoutLinkedParcelOrBuildingUnit.Table} ");

            builder.HasIndex(x => x.AddressPersistentLocalId);
            builder.HasIndex(x => x.NisCode);
        }
    }
}
