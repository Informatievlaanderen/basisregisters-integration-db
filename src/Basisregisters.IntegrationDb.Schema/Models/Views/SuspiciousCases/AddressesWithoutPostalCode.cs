namespace Basisregisters.IntegrationDb.Schema.Models.Views.SuspiciousCases
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class AddressesWithoutPostalCode
    {
        public int AddressPersistentLocalId { get; set; }
        public int NisCode { get; set; }
        public DateTimeOffset Timestamp { get; set; }

        public AddressesWithoutPostalCode() { }
    }

    public sealed class AddressesWithoutPostalCodeConfiguration : IEntityTypeConfiguration<AddressesWithoutPostalCode>
    {
        public void Configure(EntityTypeBuilder<AddressesWithoutPostalCode> builder)
        {
            builder
                .ToView(nameof(AddressesWithoutPostalCode), IntegrationContext.Schema)
                .HasNoKey()
                .ToSqlQuery(@$"
                            SELECT
                                ""AddressPersistentLocalId"",
                                ""NisCode"",
                                ""Timestamp""
                            FROM  {Views.AddressesWithoutPostalCode.Table} ");

            builder.HasIndex(x => x.AddressPersistentLocalId);
        }
    }
}
