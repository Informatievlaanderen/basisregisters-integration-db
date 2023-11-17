namespace Basisregisters.IntegrationDb.Schema.Models.Views
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class ParcelAddressRelations
    {
        public string CaPaKey { get; set; }
        public string AddressId { get; set; }

        public ParcelAddressRelations() { }
    }

    public sealed class ParcelAddressRelationConfiguration : IEntityTypeConfiguration<ParcelAddressRelations>
    {
        public void Configure(EntityTypeBuilder<ParcelAddressRelations> builder)
        {
            builder
                .ToView(nameof(ParcelAddressRelations), IntegrationContext.Schema)
                .HasNoKey()
                .ToSqlQuery(@$"select ""CaPaKey"", ""AddressId"" FROM ""{IntegrationContext.Schema}"".""{nameof(ViewQueries.VIEW_ParcelAddressRelations)}""; ");
        }
    }
}
