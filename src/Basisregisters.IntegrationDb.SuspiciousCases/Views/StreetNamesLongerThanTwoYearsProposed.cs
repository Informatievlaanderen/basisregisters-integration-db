namespace Basisregisters.IntegrationDb.SuspiciousCases.Views
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public sealed class StreetNamesLongerThanTwoYearsProposed : SuspiciousCase
    {
        public int StreetNamePersistentLocalId { get; set; }
        public override Category Category => Category.StreetName;
    }

    public sealed class StreetNamesLongerThanTwoYearsProposedConfiguration : IEntityTypeConfiguration<StreetNamesLongerThanTwoYearsProposed>
    {
        public void Configure(EntityTypeBuilder<StreetNamesLongerThanTwoYearsProposed> builder)
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
                            FROM  {Schema}.{ViewName}");

            builder.Property(x => x.PersistentLocalId).HasColumnName("persistent_local_id");
            builder.Property(x => x.StreetNamePersistentLocalId).HasColumnName("streetname_persistent_local_id");
            builder.Property(x => x.NisCode).HasColumnName("nis_code");
            builder.Property(x => x.Description).HasColumnName("description");
        }

        public const string Schema = SuspiciousCasesContext.SchemaSuspiciousCases;
        public const string ViewName = "view_streetname_longer_than_two_years_proposed";

        public const string Create = $@"
            CREATE MATERIALIZED VIEW IF NOT EXISTS {Schema}.{ViewName} AS
	        SELECT
		        CAST(streetname.persistent_local_id as varchar) AS persistent_local_id,
		        streetname.persistent_local_id AS streetname_persistent_local_id,
		        streetname.nis_code AS nis_code,
		        streetname.name_dutch AS description,
		        CURRENT_TIMESTAMP AS timestamp
	        FROM integration_streetname.streetname_latest_items AS streetname
	        WHERE streetname.status = 0
	        AND streetname.is_removed = false
	        AND streetname.version_timestamp <= CURRENT_TIMESTAMP - INTERVAL '2 years';

            CREATE INDEX ix_{ViewName}_streetname_persistent_local_id ON {Schema}.{ViewName} USING btree (streetname_persistent_local_id);
            CREATE INDEX ix_{ViewName}_nis_code ON {Schema}.{ViewName} USING btree (nis_code);
            ";
    }
}
