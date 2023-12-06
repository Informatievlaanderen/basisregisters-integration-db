namespace Basisregisters.IntegrationDb.Schema.Models.Views
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using NetTopologySuite.Geometries;

    public class ActiveAddressWithoutLinkedParcels
    {
        public int AddressPersistentLocalId { get; set; }
        public int NisCode { get; set; }
        public DateTimeOffset Timestamp { get; set; }

        public ActiveAddressWithoutLinkedParcels() { }
    }

    public sealed class ActiveHouseNumberWithoutLinkedParcelConfiguration : IEntityTypeConfiguration<ActiveAddressWithoutLinkedParcels>
    {
        public void Configure(EntityTypeBuilder<ActiveAddressWithoutLinkedParcels> builder)
        {
            builder
                .ToView(nameof(ActiveAddressWithoutLinkedParcels), IntegrationContext.Schema)
                .HasNoKey()
                .ToSqlQuery(@$"select
                                ""AddressPersistentLocalId""
                                ,""NisCode""
                                ,""Timestamp""
                            FROM ""Integration"".""{ViewQueries.VIEW_ActiveAddressWithoutLinkedParcels}"" ");

            builder.HasIndex(x => x.AddressPersistentLocalId);
            builder.HasIndex(x => x.NisCode);
        }
    }
}
