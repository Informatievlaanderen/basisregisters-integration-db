namespace Basisregisters.IntegrationDb.Schema.Models.Views
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class BuildingUnitAddressRelations
    {
        public int BuildingUnitPersistentLocalId { get; set; }
        public string AddressPersistentLocalId { get; set; }

        public BuildingUnitAddressRelations() { }
    }

    public sealed class BuildingUnitAddressRelationConfiguration : IEntityTypeConfiguration<BuildingUnitAddressRelations>
    {
        public void Configure(EntityTypeBuilder<BuildingUnitAddressRelations> builder)
        {
            builder
                .ToView(nameof(BuildingUnitAddressRelations), IntegrationContext.Schema)
                .HasNoKey()
                .ToSqlQuery(@$"
                            SELECT
                                ""BuildingUnitPersistentLocalId"",
                                ""AddressPersistentLocalId""
                            FROM ""{IntegrationContext.Schema}"".""{nameof(ViewQueries.VIEW_BuildingUnitAddressRelations)}"" ");

            builder.HasIndex(x => x.BuildingUnitPersistentLocalId);
            builder.HasIndex(x => x.AddressPersistentLocalId);
        }
    }
}
