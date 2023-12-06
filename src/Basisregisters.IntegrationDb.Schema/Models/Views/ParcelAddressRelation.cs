namespace Basisregisters.IntegrationDb.Schema.Models.Views
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class ParcelAddressRelation
    {
        public string CaPaKey { get; set; }
        public int? AddressPersistentLocalId { get; set; }
        public DateTime Timestamp { get; set; }

        public ParcelAddressRelation() { }
    }

    public sealed class ParcelAddressRelationConfiguration : IEntityTypeConfiguration<ParcelAddressRelation>
    {
        public void Configure(EntityTypeBuilder<ParcelAddressRelation> builder)
        {
            builder
                .ToView("ParcelAddressRelations", IntegrationContext.Schema)
                .HasNoKey()
                .ToSqlQuery(@$"
                            select
                                ""CaPaKey"",
                                ""AddressPersistentLocalId""
                            FROM ""{IntegrationContext.Schema}"".""{nameof(ViewQueries.VIEW_ParcelAddressRelations)}"" ");

            builder.HasIndex(x => x.CaPaKey);
            builder.HasIndex(x => x.AddressPersistentLocalId);
        }
    }
}
