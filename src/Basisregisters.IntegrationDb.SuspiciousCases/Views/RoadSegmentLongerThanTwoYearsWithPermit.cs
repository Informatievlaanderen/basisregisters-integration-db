namespace Basisregisters.IntegrationDb.SuspiciousCases.Views
{
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class RoadSegmentLongerThanTwoYearsWithPermit : SuspiciousCase
    {
        public int RoadSegmentPersistentLocalId { get; set; }

        public override Category Category => Category.RoadSegment;
    }

    public sealed class RoadSegmentLongerThanTwoYearsWithPermitConfiguration : IEntityTypeConfiguration<RoadSegmentLongerThanTwoYearsWithPermit>
    {
        public void Configure(EntityTypeBuilder<RoadSegmentLongerThanTwoYearsWithPermit> builder)
        {
            builder
                .ToView(ViewName, Schema.SuspiciousCases)
                .HasNoKey()
                .ToSqlQuery(@$"
                            SELECT
                                persistent_local_id,
                                road_segment_persistent_local_id,
                                nis_code,
                                CONCAT('Wegsegment-', road_segment_persistent_local_id) as description
                            FROM {Schema.SuspiciousCases}.{ViewName}");

            builder.Property(x => x.PersistentLocalId).HasColumnName("persistent_local_id");
            builder.Property(x => x.RoadSegmentPersistentLocalId).HasColumnName("road_segment_persistent_local_id");
            builder.Property(x => x.NisCode).HasColumnName("nis_code");
            builder.Property(x => x.Description).HasColumnName("description");
        }

        public const string ViewName = "view_road_segment_longer_than_two_years_with_permit";

        public const string Create = $@"
            CREATE OR REPLACE VIEW {Schema.SuspiciousCases}.{ViewName} AS
            SELECT
                CAST(rs.id as varchar) AS persistent_local_id,
                rs.id AS road_segment_persistent_local_id,
                rs.maintainer_id AS nis_code
            FROM {SchemaLatestItems.RoadSegment} rs
            WHERE rs.status_id in (1, 2)
            AND rs.is_removed = false
            AND rs.version_timestamp <= CURRENT_TIMESTAMP - INTERVAL '2 years';";
    }
}
