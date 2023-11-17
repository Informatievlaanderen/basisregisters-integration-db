namespace Basisregisters.IntegrationDb.Schema.Models.Views
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class BuildingUnitAddressRelations
    {
        public int PersistentLocalId { get; set; }
        public string AddressId { get; set; }

        public BuildingUnitAddressRelations() { }
    }

    public sealed class BuildingUnitAddressRelationConfiguration : IEntityTypeConfiguration<BuildingUnitAddressRelations>
    {
        public void Configure(EntityTypeBuilder<BuildingUnitAddressRelations> builder)
        {
            builder
                .ToView(nameof(BuildingUnitAddressRelations), IntegrationContext.Schema)
                .HasNoKey()
                .ToSqlQuery(@$"select ""PersistentLocalId"", ""AddressId"" FROM ""{IntegrationContext.Schema}"".""{nameof(ViewQueries.VIEW_BuildingUnitAddressRelations)}""; ");
        }
    }
}
