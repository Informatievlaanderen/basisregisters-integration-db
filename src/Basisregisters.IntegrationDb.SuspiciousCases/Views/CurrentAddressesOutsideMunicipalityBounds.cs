namespace Basisregisters.IntegrationDb.SuspiciousCases.Views
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class CurrentAddressesOutsideMunicipalityBounds : SuspiciousCase
    {
        public int AddressPersistentLocalId { get; set; }
        public int NisCode { get; set; }
        public override Category Category => Category.Address;

        public CurrentAddressesOutsideMunicipalityBounds()
        { }
    }

    public sealed class CurrentAddressesOutsideMunicipalityBoundsConfiguration : IEntityTypeConfiguration<CurrentAddressesOutsideMunicipalityBounds>
    {
        public void Configure(EntityTypeBuilder<CurrentAddressesOutsideMunicipalityBounds> builder)
        {
            builder
                .ToView(ViewName, Schema)
                .HasNoKey()
                .ToSqlQuery(@$"
                            SELECT
                               persistent_local_id,
                                streetname_persistent_local_id,
                                nis_code,
                                description
                            FROM  {Schema}.{ViewName} ");

            builder.Property(x => x.PersistentLocalId).HasColumnName("persistent_local_id");
            builder.Property(x => x.AddressPersistentLocalId).HasColumnName("address_persistent_local_id");
            builder.Property(x => x.NisCode).HasColumnName("nis_code");
            builder.Property(x => x.Description).HasColumnName("description");
        }

        public const string Schema = SuspiciousCasesContext.SchemaSuspiciousCases;
        public const string ViewName = "view_current_addresses_outside_municipality_bounds";

        public const string Create = $@"
            CREATE MATERIALIZED VIEW IF NOT EXISTS {Schema}.{ViewName} AS
            SELECT
		        CAST(a.persistent_local_id as varchar) AS persistent_local_id,
                a.persistent_local_id AS address_persistent_local_id,
                mg.nis_code,
		        CURRENT_TIMESTAMP AS timestamp
            FROM integration_municipality.municipality_geometries mg
            JOIN integration_streetname.streetname_latest_items sn
                ON sn.nis_code = mg.nis_code
            JOIN integration_addresses.address_latest_items a
                ON a.street_name_persistent_local_id = sn.persistent_local_id
            WHERE ST_Within(a.geometry, mg.geometry) IS FALSE
            AND a.status = 2
            AND a.removed = false;

            CREATE INDEX ix_{ViewName}_address_persistent_local_id ON {Schema}.{ViewName} USING btree (address_persistent_local_id);
            CREATE INDEX ix_{ViewName}_nis_code ON {Schema}.{ViewName} USING btree (nis_code);
            ";
    }
}
