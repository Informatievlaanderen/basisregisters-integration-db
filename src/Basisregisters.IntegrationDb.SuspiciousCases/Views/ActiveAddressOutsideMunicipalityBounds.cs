﻿namespace Basisregisters.IntegrationDb.SuspiciousCases.Views
{
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class ActiveAddressOutsideMunicipalityBounds : SuspiciousCase
    {
        public int AddressPersistentLocalId { get; set; }

        public override Category Category => Category.Address;
    }

    public sealed class ActiveAddressOutsideMunicipalityBoundsConfiguration : IEntityTypeConfiguration<ActiveAddressOutsideMunicipalityBounds>
    {
        public void Configure(EntityTypeBuilder<ActiveAddressOutsideMunicipalityBounds> builder)
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

        public const string ViewName = "view_active_address_outside_municipality_bounds";

        public const string Create = $@"
            DROP VIEW IF EXISTS {Schema.SuspiciousCases}.{ViewName};
            CREATE VIEW {Schema.SuspiciousCases}.{ViewName} AS
            SELECT
                CAST(a.persistent_local_id as varchar) AS persistent_local_id,
                a.persistent_local_id as address_persistent_local_id,
                s.nis_code,
                {Schema.FullAddress}(s.name_dutch, a.house_number, a.box_number, a.postal_code, m.name_dutch) as description
            FROM {SchemaLatestItems.Address} a
            JOIN {SchemaLatestItems.StreetName} s ON s.persistent_local_id = a.street_name_persistent_local_id
            JOIN {SchemaLatestItems.Municipality} m ON s.municipality_id = m.municipality_id
            LEFT JOIN {SchemaLatestItems.MunicipalityGeometries} mg ON m.nis_code = mg.nis_code AND ST_Within(a.geometry, mg.geometry)
            WHERE
                a.removed = false
                AND a.status in (1, 2)
                AND mg.nis_code IS NULL
            ;";
    }
}
