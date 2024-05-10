namespace Basisregisters.IntegrationDb.SuspiciousCases.Views.Internal
{
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class ActiveBuildingUnitWithoutAddress : SuspiciousCase
    {
        public int BuildingUnitPersistentLocalId { get; set; }

        public override Category Category => Category.BuildingUnit;
    }

    public sealed class ActiveBuildingUnitWithoutAddressConfiguration : IEntityTypeConfiguration<ActiveBuildingUnitWithoutAddress>
    {
        public void Configure(EntityTypeBuilder<ActiveBuildingUnitWithoutAddress> builder)
        {
            builder
                .ToView(ViewName, Schema.SuspiciousCases)
                .HasNoKey()
                .ToSqlQuery(@$"
                            SELECT
                                persistent_local_id,
                                building_unit_persistent_local_id,
                                nis_code,
                                CONCAT('Gebouweenheid-',  building_unit_persistent_local_id) as description
                            FROM {Schema.SuspiciousCases}.{ViewName}");

            builder.Property(x => x.PersistentLocalId).HasColumnName("persistent_local_id");
            builder.Property(x => x.BuildingUnitPersistentLocalId).HasColumnName("building_unit_persistent_local_id");
            builder.Property(x => x.NisCode).HasColumnName("nis_code");
            builder.Property(x => x.Description).HasColumnName("description");
        }

        public const string ViewName = "view_active_building_unit_without_address";

        public const string Create = $@"
            CREATE VIEW {Schema.SuspiciousCases}.{ViewName} AS
                SELECT
		            CAST(bu.building_unit_persistent_local_id as varchar) AS persistent_local_id,
                    bu.building_unit_persistent_local_id,
                    b.nis_code
                FROM {SchemaLatestItems.BuildingUnit} AS bu
                JOIN {SchemaLatestItems.Building} AS b
                    ON bu.building_persistent_local_id = b.building_persistent_local_id
                LEFT OUTER JOIN {SchemaLatestItems.BuildingUnitAddresses} AS rel
                    ON bu.building_unit_persistent_local_id = rel.building_unit_persistent_local_id
                WHERE
                    bu.is_removed IS FALSE
                    AND bu.status in ('Planned', 'Realized')
                    AND rel.address_persistent_local_id IS NULL
                ;";
    }
}
