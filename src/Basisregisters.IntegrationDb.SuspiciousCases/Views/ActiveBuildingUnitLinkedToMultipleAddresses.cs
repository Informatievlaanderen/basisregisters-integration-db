namespace Basisregisters.IntegrationDb.SuspiciousCases.Views
{
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class ActiveBuildingUnitLinkedToMultipleAddresses : SuspiciousCase
    {
        public int BuildingUnitPersistentLocalId { get; set; }

        public override Category Category => Category.BuildingUnit;
    }

    public sealed class ActiveBuildingUnitLinkedToMultipleAddressesConfiguration : IEntityTypeConfiguration<ActiveBuildingUnitLinkedToMultipleAddresses>
    {
        public void Configure(EntityTypeBuilder<ActiveBuildingUnitLinkedToMultipleAddresses> builder)
        {
            builder
                .ToView(ViewName, Schema.SuspiciousCases)
                .HasNoKey()
                .ToSqlQuery(@$"SELECT
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

        public const string ViewName = "view_building_unit_linked_to_multiple_addresses";

        public const string Create = $@"
            DROP VIEW IF EXISTS {Schema.SuspiciousCases}.{ViewName};
            CREATE VIEW {Schema.SuspiciousCases}.{ViewName} AS
            SELECT
                CAST(bu.building_unit_persistent_local_id as varchar) AS persistent_local_id,
                bu.building_unit_persistent_local_id,
                b.nis_code
            FROM {SchemaLatestItems.BuildingUnit} AS bu
            INNER JOIN {SchemaLatestItems.Building} AS b ON bu.building_persistent_local_id = b.building_persistent_local_id
            JOIN(
                SELECT
                    rel.building_unit_persistent_local_id,
                    COUNT(*) AS linked_building_units
                FROM {SchemaLatestItems.BuildingUnitAddresses} rel
                GROUP BY
                    rel.building_unit_persistent_local_id
                HAVING
                    COUNT(*) > 1) c ON c.building_unit_persistent_local_id = bu.building_unit_persistent_local_id
            WHERE
                bu.is_removed IS FALSE
                AND bu.status IN ('Planned','Realized')
            ;";
    }
}
