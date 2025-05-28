namespace Basisregisters.IntegrationDb.SuspiciousCases.Views
{
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public sealed class BuildingUnitsLongerThanTwoYearsPlanned : SuspiciousCase
    {
        public int BuildingUnitPersistentLocalId { get; set; }

        public override Category Category => Category.BuildingUnit;
    }

    public sealed class BuildingUnitsLongerThanTwoYearsPlannedConfiguration : IEntityTypeConfiguration<BuildingUnitsLongerThanTwoYearsPlanned>
    {
        public void Configure(EntityTypeBuilder<BuildingUnitsLongerThanTwoYearsPlanned> builder)
        {
            builder
                .ToView(ViewName, Schema.SuspiciousCases)
                .HasNoKey()
                .ToSqlQuery(@$"
                            SELECT
                                persistent_local_id,
                                building_unit_persistent_local_id,
                                nis_code,
                                CONCAT('Gebouweenheden-',  building_unit_persistent_local_id) as description
                            FROM  {Schema.SuspiciousCases}.{ViewName}");

            builder.Property(x => x.PersistentLocalId).HasColumnName("persistent_local_id");
            builder.Property(x => x.BuildingUnitPersistentLocalId).HasColumnName("building_unit_persistent_local_id");
            builder.Property(x => x.NisCode).HasColumnName("nis_code");
            builder.Property(x => x.Description).HasColumnName("description");
        }

        public const string ViewName = "view_building_unit_longer_than_two_years_planned";

        public const string Create = $@"
            DROP VIEW IF EXISTS {Schema.SuspiciousCases}.{ViewName};
            CREATE VIEW {Schema.SuspiciousCases}.{ViewName} AS
	        SELECT
		        CAST(bu.building_unit_persistent_local_id as varchar) AS persistent_local_id,
		        bu.building_unit_persistent_local_id AS building_unit_persistent_local_id,
		        b.nis_code AS nis_code
	        FROM {SchemaLatestItems.BuildingUnit} AS bu
            JOIN {SchemaLatestItems.Building} b ON b.building_persistent_local_id = bu.building_persistent_local_id
	        WHERE bu.status = 'Planned'
	        AND bu.is_removed = false
	        AND bu.version_timestamp <= CURRENT_TIMESTAMP - INTERVAL '2 years'
            ;";
    }
}
