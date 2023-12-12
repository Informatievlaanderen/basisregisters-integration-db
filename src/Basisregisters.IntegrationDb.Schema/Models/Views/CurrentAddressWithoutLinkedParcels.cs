namespace Basisregisters.IntegrationDb.Schema.Models.Views
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using NetTopologySuite.Geometries;

    public class CurrentAddressWithoutLinkedParcels
    {
        public int AddressPersistentLocalId { get; set; }
        public int NisCode { get; set; }
        public DateTimeOffset Timestamp { get; set; }

        public CurrentAddressWithoutLinkedParcels() { }
    }

    public sealed class CurrentAddressWithoutLinkedParcelConfiguration : IEntityTypeConfiguration<CurrentAddressWithoutLinkedParcels>
    {
        public void Configure(EntityTypeBuilder<CurrentAddressWithoutLinkedParcels> builder)
        {
            builder
                .ToView(nameof(CurrentAddressWithoutLinkedParcels), IntegrationContext.Schema)
                .HasNoKey()
                .ToSqlQuery(@$"SELECT
                                ""AddressPersistentLocalId""
                                ,""NisCode""
                                ,""Timestamp""
                            FROM  {Views.CurrentAddressWithoutLinkedParcels.Table} ");

            builder.HasIndex(x => x.AddressPersistentLocalId);
            builder.HasIndex(x => x.NisCode);
        }
    }
}
