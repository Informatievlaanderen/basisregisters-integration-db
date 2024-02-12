namespace Basisregisters.IntegrationDb.SuspiciousCases.Views
{
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public sealed class BuildingsLongerThanTwoYearsPlanned : SuspiciousCase
    {
        public int BuildingPersistentLocalId { get; set; }

        public override Category Category => Category.Building;
    }

    public sealed class BuildingsLongerThanTwoYearsPlannedConfiguration : IEntityTypeConfiguration<BuildingsLongerThanTwoYearsPlanned>
    {
        public void Configure(EntityTypeBuilder<BuildingsLongerThanTwoYearsPlanned> builder)
        {
            builder
                .ToView(ViewName, Schema.SuspiciousCases)
                .HasNoKey()
                .ToSqlQuery(@$"
                            SELECT
                                persistent_local_id,
                                building_persistent_local_id,
                                nis_code,
                                CONCAT('Gebouw-',  building_persistent_local_id) as description
                            FROM  {Schema.SuspiciousCases}.{ViewName}");

            builder.Property(x => x.PersistentLocalId).HasColumnName("persistent_local_id");
            builder.Property(x => x.BuildingPersistentLocalId).HasColumnName("building_persistent_local_id");
            builder.Property(x => x.NisCode).HasColumnName("nis_code");
            builder.Property(x => x.Description).HasColumnName("description");
        }

        public const string ViewName = "view_buildings_longer_than_two_years_planned";

        public const string Create = $@"
            CREATE VIEW {Schema.SuspiciousCases}.{ViewName} AS
	        SELECT
		        CAST(b.building_persistent_local_id as varchar) AS persistent_local_id,
		        b.building_persistent_local_id AS building_persistent_local_id,
		        b.nis_code AS nis_code
	        FROM {SchemaLatestItems.Building} AS b
	        WHERE b.status = 'Planned'
	        AND b.is_removed = false
	        AND b.version_timestamp <= CURRENT_TIMESTAMP - INTERVAL '2 years';
            ";
    }
}
