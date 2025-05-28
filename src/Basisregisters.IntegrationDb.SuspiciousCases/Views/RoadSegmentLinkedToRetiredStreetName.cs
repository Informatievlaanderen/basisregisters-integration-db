namespace Basisregisters.IntegrationDb.SuspiciousCases.Views
{
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class RoadSegmentLinkedToRetiredStreetName : SuspiciousCase
    {
        public int RoadSegmentPersistentLocalId { get; set; }

        public override Category Category => Category.RoadSegment;
    }

    public sealed class RoadSegmentLinkedToRetiredStreetNameConfiguration : IEntityTypeConfiguration<RoadSegmentLinkedToRetiredStreetName>
    {
        public void Configure(EntityTypeBuilder<RoadSegmentLinkedToRetiredStreetName> builder)
        {
            builder
                .ToView(ViewName, Schema.SuspiciousCases)
                .HasNoKey()
                .ToSqlQuery(@$"
                            SELECT
                                persistent_local_id,
                                road_segment_persistent_local_id,
                                nis_code,
                                description
                            FROM {Schema.SuspiciousCases}.{ViewName}");

            builder.Property(x => x.PersistentLocalId).HasColumnName("persistent_local_id");
            builder.Property(x => x.RoadSegmentPersistentLocalId).HasColumnName("road_segment_persistent_local_id");
            builder.Property(x => x.NisCode).HasColumnName("nis_code");
            builder.Property(x => x.Description).HasColumnName("description");
        }

        public const string ViewName = "view_road_segment_linked_to_retired_streetname";

        public const string Create = $@"
            DROP VIEW IF EXISTS {Schema.SuspiciousCases}.{ViewName};
            CREATE VIEW {Schema.SuspiciousCases}.{ViewName} AS
            SELECT
                CAST(rs.id as varchar) AS persistent_local_id,
                rs.id AS road_segment_persistent_local_id,
                leftMuni.nis_code AS nis_code,
                leftStreetName.name_dutch AS description
            FROM {SchemaLatestItems.RoadSegment} rs
            JOIN {SchemaLatestItems.StreetName} as leftStreetName on
                rs.left_side_street_name_id = leftStreetName.persistent_local_id
                and leftStreetName.status in (2, 3)
                and leftStreetName.is_removed = false
            JOIN {SchemaLatestItems.Municipality} as leftMuni on
                rs.maintainer_id = leftMuni.nis_code
                and leftMuni.is_removed = false
            WHERE
                rs.is_removed = false
            UNION
            SELECT
                CAST(rs.id as varchar) AS persistent_local_id,
                rs.id AS road_segment_persistent_local_id,
                rightMuni.nis_code AS nis_code,
                rightStreetName.name_dutch AS description
            FROM {SchemaLatestItems.RoadSegment} rs
            JOIN {SchemaLatestItems.StreetName} as rightStreetName on
                rs.right_side_street_name_id = rightStreetName.persistent_local_id
                and rightStreetName.status in (2, 3)
                and rightStreetName.is_removed = false
            JOIN {SchemaLatestItems.Municipality} as rightMuni on
                rs.maintainer_id = rightMuni.nis_code
                and rightMuni.is_removed = false
            WHERE
                rs.is_removed = false
            ;";
    }
}
