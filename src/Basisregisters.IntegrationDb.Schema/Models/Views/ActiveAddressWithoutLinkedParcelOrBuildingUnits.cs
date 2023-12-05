namespace Basisregisters.IntegrationDb.Schema.Models.Views
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class ActiveAddressWithoutLinkedParcelOrBuildingUnits
    {
        public int PersistentLocalId { get; set; }
        public int NisCode { get; set; }
        public DateTimeOffset Timestamp { get; set; }

        public ActiveAddressWithoutLinkedParcelOrBuildingUnits() { }
    }

    public sealed class ActiveAddressWithoutLinkedParcelOrBuildingUnitsConfiguration : IEntityTypeConfiguration<ActiveAddressWithoutLinkedParcelOrBuildingUnits>
    {
        public void Configure(EntityTypeBuilder<ActiveAddressWithoutLinkedParcelOrBuildingUnits> builder)
        {
            builder
                .ToView("ActiveAddressWithoutLinkedParcelOrBuildings", IntegrationContext.Schema)
                .HasNoKey()
                .ToSqlQuery(@$"select
                                ""PersistentLocalId"",
                                ""NisCode"",
                                ""Timestamp""
                                FROM ""Integration"".""{nameof(ViewQueries.VIEW_ActiveAddressWithoutLinkedParcelOrBuildingUnit)}"" ");

            builder.HasIndex(x => x.PersistentLocalId);
            builder.HasIndex(x => x.NisCode);
        }
    }
}
