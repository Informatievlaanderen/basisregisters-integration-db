namespace Basisregisters.IntegrationDb.Schema.Models.Views
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class ParcelsLinkedToMultipleAddresses
    {
        public string CaPaKey { get; set; }
        public string FullName { get; set; }
        public string Addresses { get; set; }

        public ParcelsLinkedToMultipleAddresses() { }
    }

    public sealed class ParcelsLinkedToMultipleAddressesConfiguration : IEntityTypeConfiguration<ParcelsLinkedToMultipleAddresses>
    {
        public void Configure(EntityTypeBuilder<ParcelsLinkedToMultipleAddresses> builder)
        {
            builder
                .ToView(nameof(ParcelsLinkedToMultipleAddresses), IntegrationContext.Schema)
                .HasNoKey()
                .ToSqlQuery(@$"select ""CaPaKey"", ""FullName"", ""Addresses"" FROM ""{IntegrationContext.Schema}"".""{nameof(ViewQueries.VIEW_ParcelsLinkedToMultipleAddresses)}"" ");
        }
    }
}
