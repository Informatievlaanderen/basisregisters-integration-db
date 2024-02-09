namespace Basisregisters.IntegrationDb.SuspiciousCases.Views
{
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class AddressesLinkedToMultipleBuildingUnits : SuspiciousCase
    {
        public int AddressPersistentLocalId { get; set; }
        public override Category Category => Category.Address;
    }

    public sealed class AddressesLinkedToMultipleBuildingUnitsConfiguration : IEntityTypeConfiguration<AddressesLinkedToMultipleBuildingUnits>
    {
        public void Configure(EntityTypeBuilder<AddressesLinkedToMultipleBuildingUnits> builder)
        {
            builder
                .ToView(ViewName, Schema.SuspiciousCases)
                .HasNoKey()
                .ToSqlQuery(@$"
                            SELECT
                                persistent_local_id,
                                address_persistent_local_id,
                                nis_code,
                                description
                            FROM {Schema.SuspiciousCases}.{ViewName}");

            builder.Property(x => x.PersistentLocalId).HasColumnName("persistent_local_id");
            builder.Property(x => x.AddressPersistentLocalId).HasColumnName("address_persistent_local_id");
            builder.Property(x => x.NisCode).HasColumnName("nis_code");
            builder.Property(x => x.Description).HasColumnName("description");
        }


        public const string ViewName = "view_addresses_linked_to_multiple_building_units";

        public const string Create = $@"
            CREATE VIEW {Schema.SuspiciousCases}.{ViewName} AS
            SELECT
		        CAST(c.address_persistent_local_id as varchar) AS persistent_local_id,
                c.address_persistent_local_id,
                b.nis_code,
                {Schema.FullAddress}(s.name_dutch, a.house_number, a.box_number, a.postal_code, m.name_dutch) as description
            FROM {SchemaLatestItems.Building} b
            LEFT OUTER JOIN {SchemaLatestItems.BuildingUnit} bu ON bu.building_persistent_local_id = b.building_persistent_local_id
            LEFT OUTER JOIN {SchemaLatestItems.BuildingUnitAddresses} rel ON rel.building_unit_persistent_local_id = bu.building_unit_persistent_local_id
            LEFT OUTER JOIN {SchemaLatestItems.Address} a ON rel.address_persistent_local_id = a.persistent_local_id
            LEFT OUTER JOIN {SchemaLatestItems.StreetName} s ON s.persistent_local_id = a.street_name_persistent_local_id
            LEFT OUTER JOIN {SchemaLatestItems.Municipality} m ON s.municipality_id = m.municipality_id
            JOIN (
            SELECT
                rel.address_persistent_local_id,
                COUNT(*) AS linked_building_units
            FROM {SchemaLatestItems.BuildingUnitAddresses} rel
            LEFT OUTER JOIN {SchemaLatestItems.Address} a ON a.persistent_local_id = rel.address_persistent_local_id
            WHERE
                a.removed = false
            AND
                a.status IN (1,2)
            GROUP BY
                rel.address_persistent_local_id
            HAVING
                COUNT(*) > 1) c ON c.address_persistent_local_id = rel.address_persistent_local_id";
    }
}
