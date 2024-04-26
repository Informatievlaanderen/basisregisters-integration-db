namespace Basisregisters.IntegrationDb.SuspiciousCases.Views
{
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class AddressLinkedWithBuildingUnitButNotWithParcel : SuspiciousCase
    {
        public int AddressPersistentLocalId { get; set; }
        public override Category Category => Category.Address;
    }

    public sealed class AddressLinkedWithBuildingUnitButNotWithParcelConfiguration
        : IEntityTypeConfiguration<AddressLinkedWithBuildingUnitButNotWithParcel>
    {
        public void Configure(EntityTypeBuilder<AddressLinkedWithBuildingUnitButNotWithParcel> builder)
        {
            builder
                .ToView(ViewName, Schema.SuspiciousCases)
                .HasNoKey()
                .ToSqlQuery(@$"SELECT
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

        public const string ViewName = "view_address_linked_with_building_unit_but_not_with_parcel";

        public const string Create = $@"
            CREATE VIEW {Schema.SuspiciousCases}.{ViewName} AS
            SELECT
                CAST(address.persistent_local_id AS varchar) AS persistent_local_id,
                address.persistent_local_id AS address_persistent_local_id,
                streetname.nis_code,
                {Schema.FullAddress}(streetname.name_dutch, address.house_number, address.box_number, address.postal_code, municipality.name_dutch) AS description
            FROM {SchemaLatestItems.BuildingUnitAddresses} AS bu_address
            LEFT JOIN {SchemaLatestItems.ParcelAddresses} AS parcel_address
                ON bu_address.address_persistent_local_id = parcel_address.address_persistent_local_id
            JOIN {SchemaLatestItems.BuildingUnit} AS bu
                ON
                    bu_address.building_unit_persistent_local_id = bu.building_unit_persistent_local_id
                    and (bu.status in ('Planned', 'Realized'))
                    and bu.is_removed = false
            JOIN {SchemaLatestItems.Address} AS address
                ON bu_address.address_persistent_local_id = address.persistent_local_id
            JOIN {SchemaLatestItems.StreetName} AS streetname
                ON address.street_name_persistent_local_id = streetname.persistent_local_id
            JOIN {SchemaLatestItems.Municipality} AS municipality
                ON streetname.municipality_id = municipality.municipality_id
            WHERE parcel_address.parcel_id is null;";
    }
}
