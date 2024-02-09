namespace Basisregisters.IntegrationDb.SuspiciousCases.Views
{
    using Infrastructure;
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
                .ToView(ViewName, Schema.SuspiciousCases)
                .HasNoKey()
                .ToSqlQuery(@$"
                            SELECT
                                persistent_local_id,
                                streetname_persistent_local_id,
                                nis_code,
                                description
                            FROM  {Schema.SuspiciousCases}.{ViewName}");

            builder.Property(x => x.PersistentLocalId).HasColumnName("persistent_local_id");
            builder.Property(x => x.StreetNamePersistentLocalId).HasColumnName("streetname_persistent_local_id");
            builder.Property(x => x.NisCode).HasColumnName("nis_code");
            builder.Property(x => x.Description).HasColumnName("description");
        }

        public const string ViewName = "view_streetname_longer_than_two_years_proposed";

        public const string Create = $@"
            CREATE VIEW {Schema.SuspiciousCases}.{ViewName} AS
	        SELECT
		        CAST(streetname.persistent_local_id as varchar) AS persistent_local_id,
		        streetname.persistent_local_id AS streetname_persistent_local_id,
		        streetname.nis_code AS nis_code,
		        streetname.name_dutch AS description
	        FROM integration_streetname.streetname_latest_items AS streetname
	        WHERE streetname.status = 0
	        AND streetname.is_removed = false
	        AND streetname.version_timestamp <= CURRENT_TIMESTAMP - INTERVAL '2 years';
            ";
    }
}
