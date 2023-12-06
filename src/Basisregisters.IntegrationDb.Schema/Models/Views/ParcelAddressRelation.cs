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

            // Must be lower case because the materialized view defaults to lower case column for aliases.
            //builder.Property(x => x.AddressPersistentLocalId).HasColumnName("addresspersistentlocalid");

            builder.HasIndex(x => x.CaPaKey);
            builder.HasIndex(x => x.AddressPersistentLocalId);
        }
    }
}
