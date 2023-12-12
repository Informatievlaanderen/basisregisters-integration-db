namespace Basisregisters.IntegrationDb.Schema.Models.Views.SuspiciousCases
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class ParcelsLinkedToMultipleAddresses
    {
        public string CaPaKey { get; set; }
        public int NisCode { get; set; }
        public int LinkedAddressCount { get; set; }
        public DateTimeOffset Timestamp { get; set; }

        public ParcelsLinkedToMultipleAddresses() { }
    }

    public sealed class ParcelsLinkedToMultipleAddressesConfiguration : IEntityTypeConfiguration<ParcelsLinkedToMultipleAddresses>
    {
        public void Configure(EntityTypeBuilder<ParcelsLinkedToMultipleAddresses> builder)
        {
            builder
                .ToView(nameof(ParcelsLinkedToMultipleAddresses), IntegrationContext.Schema)
                .HasNoKey()
                .ToSqlQuery(@$"
                            SELECT
                                ""CaPaKey"",
                                ""NisCode"",
                                ""LinkedAddressCount"",
                                ""Timestamp""
                            FROM  {Views.ParcelsLinkedToMultipleAddresses.Table} ");
        }
    }
}
