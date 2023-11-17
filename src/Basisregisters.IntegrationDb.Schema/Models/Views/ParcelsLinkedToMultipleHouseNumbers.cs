namespace Basisregisters.IntegrationDb.Schema.Models.Views
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class ParcelsLinkedToMultipleHouseNumbers
    {
        public string CaPaKey { get; set; }
        public string FullName { get; set; }
        public string Addresses { get; set; }

        public ParcelsLinkedToMultipleHouseNumbers() { }
    }

    public sealed class ParcelsLinkedToMultipleHouseNumbersConfiguration : IEntityTypeConfiguration<ParcelsLinkedToMultipleHouseNumbers>
    {
        public void Configure(EntityTypeBuilder<ParcelsLinkedToMultipleHouseNumbers> builder)
        {
            builder
                .ToView(nameof(ParcelsLinkedToMultipleHouseNumbers), IntegrationContext.Schema)
                .HasNoKey()
                .ToSqlQuery(@$"select ""CaPaKey"", ""FullName"", ""Addresses"" FROM ""{IntegrationContext.Schema}"".""{nameof(ViewQueries.VIEW_ParcelsLinkedToMultipleHouseNumbers)}""; ");
        }
    }
}
