namespace Basisregisters.IntegrationDb.SuspiciousCases.Views
{
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class MeasuredRoadSegmentWithNoOrSingleLinkedStreetName : SuspiciousCase
    {
        public int RoadSegmentPersistentLocalId { get; set; }

        public override Category Category => Category.RoadSegment;
    }

    public sealed class MeasuredRoadSegmentWithNoOrSingleLinkedStreetNameConfiguration : IEntityTypeConfiguration<MeasuredRoadSegmentWithNoOrSingleLinkedStreetName>
    {
        public void Configure(EntityTypeBuilder<MeasuredRoadSegmentWithNoOrSingleLinkedStreetName> builder)
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

        public const string ViewName = "view_measured_road_segment_with_no_or_single_linked_streetname";

        public const string Create = $@"
            CREATE VIEW {Schema.SuspiciousCases}.{ViewName} AS
            SELECT
                CAST(rs.id as varchar) AS persistent_local_id,
                rs.id AS road_segment_persistent_local_id,
                rs.maintainer_id AS nis_code
            FROM {SchemaLatestItems.RoadSegment} rs
            WHERE
                rs.is_removed = false
                and rs.method_id = 2
                and (left_side_street_name_id = -8 or right_side_street_name_id = -8)
            ;";
    }
}
