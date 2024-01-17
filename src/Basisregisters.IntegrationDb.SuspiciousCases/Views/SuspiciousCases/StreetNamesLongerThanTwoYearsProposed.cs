namespace Basisregisters.IntegrationDb.Schema.Views.SuspiciousCases
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class StreetNamesLongerThanTwoYearsProposed
    {
        public int PersistentLocalId { get; set; }
        public string NisCode { get; set; }
        public DateTimeOffset Timestamp { get; set; }

        public StreetNamesLongerThanTwoYearsProposed()
        { }
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
                                streetname_persistent_local_id,
                                nis_code,
                                timestamp
                            FROM  {Schema}.{ViewName}");

            builder.Property(x => x.PersistentLocalId)
                .HasColumnName("streetname_persistent_local_id");
            builder.Property(x => x.NisCode)
                .HasColumnName("nis_code");
            builder.Property(x => x.Timestamp)
                .HasColumnName("timestamp");
        }

        public const string Schema = SuspiciousCasesContext.SchemaSuspiciousCases;
        public const string ViewName = "view_streetname_longer_than_two_years_proposed";

        public const string Create = $@"
            CREATE MATERIALIZED VIEW IF NOT EXISTS {Schema}.{ViewName} AS
	        SELECT
		        streetname.persistent_local_id AS persistent_local_id,
		        streetname.nis_code AS nis_code,
		        CURRENT_TIMESTAMP AS timestamp
	        FROM integration_streetname.streetname_latest_items AS streetname
	        WHERE streetname.status = 0
	        AND streetname.is_removed = false
	        AND streetname.version_timestamp <= CURRENT_TIMESTAMP - INTERVAL '2 years';

            CREATE INDEX ix_{ViewName}_persistent_local_id ON {Schema}.{ViewName} USING btree (persistent_local_id);
            CREATE INDEX ix_{ViewName}_nis_code ON {Schema}.{ViewName} USING btree (nis_code);
            ";
    }
}
