namespace Basisregisters.IntegrationDb.SuspiciousCases.Views
{
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class ProposedAddressWithoutLinkedParcelOrBuildingUnit : SuspiciousCase
    {
        public int AddressPersistentLocalId { get; set; }
        public override Category Category => Category.Address;
    }

    public sealed class
        ProposedAddressWithoutLinkedParcelOrBuildingUnitsConfiguration : IEntityTypeConfiguration<ProposedAddressWithoutLinkedParcelOrBuildingUnit>
    {
        public void Configure(EntityTypeBuilder<ProposedAddressWithoutLinkedParcelOrBuildingUnit> builder)
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

        public const string ViewName = "view_proposed_address_without_linked_parcel_or_building_unit";

        public const string Create = $@"
            CREATE VIEW {Schema.SuspiciousCases}.{ViewName} AS
                SELECT
                    CAST(a.persistent_local_id as varchar) AS persistent_local_id,
                    a.persistent_local_id AS address_persistent_local_id,
                    s.nis_code,
                    {Schema.FullAddress}(s.name_dutch, a.house_number, a.box_number, a.postal_code, m.name_dutch) as description
                FROM {SchemaLatestItems.Address} a
                LEFT OUTER JOIN {SchemaLatestItems.StreetName} s ON s.persistent_local_id = a.street_name_persistent_local_id
                LEFT OUTER JOIN {SchemaLatestItems.Municipality} m ON s.municipality_id = m.municipality_id
                WHERE EXISTS (
                    SELECT 1
                    FROM {SchemaLatestItems.Address} AS address
                    LEFT JOIN {SchemaLatestItems.ParcelAddresses} AS pa
                        ON address.persistent_local_id = pa.address_persistent_local_id
                    LEFT JOIN {SchemaLatestItems.BuildingUnitAddresses} AS ba
                        ON address.persistent_local_id = ba.address_persistent_local_id
                    WHERE address.persistent_local_id = a.persistent_local_id
                        AND (pa.address_persistent_local_id IS NULL AND ba.address_persistent_local_id IS NULL)
                        AND address.status = 1
                        AND address.removed = false
                        AND address.position_specification != 6)
            ";
    }
}
