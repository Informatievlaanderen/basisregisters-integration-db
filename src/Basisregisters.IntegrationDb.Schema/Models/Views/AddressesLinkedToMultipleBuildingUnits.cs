namespace Basisregisters.IntegrationDb.Schema.Models.Views
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class AddressesLinkedToMultipleBuildingUnits
    {
        public int PersistentLocalId { get; set; }
        public string FullName { get; set; }
        public string Status { get; set; }

        public AddressesLinkedToMultipleBuildingUnits() { }
    }

    public sealed class AddressesLinkedToMultipleBuildingUnitsConfiguration : IEntityTypeConfiguration<AddressesLinkedToMultipleBuildingUnits>
    {
        public void Configure(EntityTypeBuilder<AddressesLinkedToMultipleBuildingUnits> builder)
        {
            builder
                .ToView(nameof(AddressesLinkedToMultipleBuildingUnits), IntegrationContext.Schema)
                .HasNoKey()
                .ToSqlQuery(@"select ""PersistentLocalId"", ""FullName"", ""Status"" FROM ""Integration"".""VIEW_AddressesLinkedToMultipleBuildingUnits""; ");

            builder.HasIndex(x => x.PersistentLocalId);
            builder.HasIndex(x => x.Status);
        }
    }
}
