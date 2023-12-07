namespace Basisregisters.IntegrationDb.Schema.Models.Views
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class AddressesWithoutPostalCode
    {
        public int AddressPersistentLocalId { get; set; }
        public int NisCode { get; set; }
        public DateTime Timestamp { get; set; }

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
                            FROM ""{IntegrationContext.Schema}"".""{nameof(ViewQueries.VIEW_AddressesWithoutPostalCode)}"" ");

            builder.HasIndex(x => x.AddressPersistentLocalId);
        }
    }
}
