namespace Basisregisters.IntegrationDb.Schema.Models.Views
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class BuildingUnitAddressRelation
    {
        public int BuildingUnitPersistentLocalId { get; set; }
        public string AddressPersistentLocalId { get; set; }

        public BuildingUnitAddressRelation() { }
    }

    public sealed class BuildingUnitAddressRelationConfiguration : IEntityTypeConfiguration<BuildingUnitAddressRelation>
    {
        public void Configure(EntityTypeBuilder<BuildingUnitAddressRelation> builder)
        {
            builder
                .ToView("BuildingUnitAddressRelations", IntegrationContext.Schema)
                .HasNoKey()
                .ToSqlQuery(@$"
                            select
                                ""BuildingUnitPersistentLocalId"",
                                ""AddressPersistentLocalId""
                            FROM ""{IntegrationContext.Schema}"".""{nameof(ViewQueries.VIEW_BuildingUnitAddressRelations)}"" ");

            builder.HasIndex(x => x.BuildingUnitPersistentLocalId);
            builder.HasIndex(x => x.AddressPersistentLocalId);
        }
    }
}
