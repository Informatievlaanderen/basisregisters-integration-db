﻿namespace Basisregisters.IntegrationDb.SuspiciousCases.Views
{
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class CurrentAddressWithoutLinkedParcelOrBuildingUnit : SuspiciousCase
    {
        public int AddressPersistentLocalId { get; set; }
        public override Category Category => Category.Address;
    }

    public sealed class CurrentAddressWithoutLinkedParcelOrBuildingUnitConfiguration : IEntityTypeConfiguration<CurrentAddressWithoutLinkedParcelOrBuildingUnit>
    {
        public void Configure(EntityTypeBuilder<CurrentAddressWithoutLinkedParcelOrBuildingUnit> builder)
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

        public const string ViewName = "view_current_address_without_linked_parcel_or_building_unit";

        public const string Create = $@"
            DROP VIEW IF EXISTS {Schema.SuspiciousCases}.{ViewName};
            CREATE VIEW {Schema.SuspiciousCases}.{ViewName} AS
                SELECT
                    CAST(a.persistent_local_id as varchar) AS persistent_local_id,
                    a.persistent_local_id AS address_persistent_local_id,
                    s.nis_code,
                    {Schema.FullAddress}(s.name_dutch, a.house_number, a.box_number, a.postal_code, m.name_dutch) as description
                FROM {SchemaLatestItems.Address} a
                JOIN {SchemaLatestItems.StreetName} s ON s.persistent_local_id = a.street_name_persistent_local_id
                JOIN {SchemaLatestItems.Municipality} m ON s.municipality_id = m.municipality_id
                LEFT JOIN {SchemaLatestItems.ParcelAddresses} AS pa
                    ON a.persistent_local_id = pa.address_persistent_local_id
                LEFT JOIN {SchemaLatestItems.BuildingUnitAddresses} AS ba
                    ON a.persistent_local_id = ba.address_persistent_local_id
                WHERE
                    a.removed = false
                    AND a.status = 2
                    AND a.position_specification != 6
                    AND pa.address_persistent_local_id is null AND ba.address_persistent_local_id is null
                ;";
    }
}
