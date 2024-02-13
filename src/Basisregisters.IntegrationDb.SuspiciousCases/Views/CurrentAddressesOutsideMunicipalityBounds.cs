namespace Basisregisters.IntegrationDb.SuspiciousCases.Views
{
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class CurrentAddressesOutsideMunicipalityBounds : SuspiciousCase
    {
        public int AddressPersistentLocalId { get; set; }

        public override Category Category => Category.Address;
    }

    public sealed class CurrentAddressesOutsideMunicipalityBoundsConfiguration : IEntityTypeConfiguration<CurrentAddressesOutsideMunicipalityBounds>
    {
        public void Configure(EntityTypeBuilder<CurrentAddressesOutsideMunicipalityBounds> builder)
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


        public const string ViewName = "view_current_address_outside_municipality_bounds";

        public const string Create = $@"
            CREATE VIEW {Schema.SuspiciousCases}.{ViewName} AS
            SELECT
                CAST(a.persistent_local_id as varchar) AS persistent_local_id,
                a.persistent_local_id as address_persistent_local_id,
                s.nis_code,
                {Schema.FullAddress}(s.name_dutch, a.house_number, a.box_number, a.postal_code, m.name_dutch) as description
            FROM {SchemaLatestItems.Address} a
            LEFT OUTER JOIN {SchemaLatestItems.StreetName} s ON s.persistent_local_id = a.street_name_persistent_local_id
            LEFT OUTER JOIN {SchemaLatestItems.Municipality} m ON s.municipality_id = m.municipality_id
            LEFT OUTER JOIN {SchemaLatestItems.MunicipalityGeometries} mg ON m.nis_code = mg.nis_code
            WHERE ST_Within(a.geometry, mg.geometry) IS FALSE
            AND a.status = 2
            AND a.removed = false
            ;";
    }
}
