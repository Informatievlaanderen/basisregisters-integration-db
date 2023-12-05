namespace Basisregisters.IntegrationDb.Schema.Models.Views
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using NetTopologySuite.Geometries;

    public class ActiveHouseNumberWithoutLinkedParcel
    {
        public int PersistentLocalId { get; set; }
        public int NisCode { get; set; }
        public DateTimeOffset Timestamp { get; set; }

        public ActiveHouseNumberWithoutLinkedParcel() { }
    }

    public sealed class ActiveHouseNumberWithoutLinkedParcelConfiguration : IEntityTypeConfiguration<ActiveHouseNumberWithoutLinkedParcel>
    {
        public void Configure(EntityTypeBuilder<ActiveHouseNumberWithoutLinkedParcel> builder)
        {
            builder
                .ToView(nameof(ActiveHouseNumberWithoutLinkedParcel), IntegrationContext.Schema)
                .HasNoKey()
                .ToSqlQuery(@$"select
                                ""PersistentLocalId""
                                ,""NisCode""
                                ,""Timestamp""
                            FROM ""Integration"".""{ViewQueries.VIEW_ActiveAddressWithoutLinkedParcels}"" ");

            builder.Property(x => x.PersistentLocalId);
            builder.Property(x => x.NisCode);
        }
    }
}
