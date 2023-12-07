namespace Basisregisters.IntegrationDb.Schema.Models.Views
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class ParcelAddressRelations
    {
        public string CaPaKey { get; set; }
        public int? AddressPersistentLocalId { get; set; }
        public DateTime Timestamp { get; set; }

        public ParcelAddressRelations() { }
    }

    public sealed class ParcelAddressRelationConfiguration : IEntityTypeConfiguration<ParcelAddressRelations>
    {
        public void Configure(EntityTypeBuilder<ParcelAddressRelations> builder)
        {
            builder
                .ToView(nameof(ParcelAddressRelations), IntegrationContext.Schema)
                .HasNoKey()
                .ToSqlQuery(@$"
                            SELECT
                                ""CaPaKey"",
                                ""AddressPersistentLocalId""
                            FROM ""{IntegrationContext.Schema}"".""{nameof(ViewQueries.VIEW_ParcelAddressRelations)}"" ");

            builder.HasIndex(x => x.CaPaKey);
            builder.HasIndex(x => x.AddressPersistentLocalId);
        }
    }
}
