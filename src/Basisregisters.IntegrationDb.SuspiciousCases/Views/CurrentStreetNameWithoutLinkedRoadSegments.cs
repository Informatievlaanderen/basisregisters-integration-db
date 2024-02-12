namespace Basisregisters.IntegrationDb.Schema.Views.SuspiciousCases
{
    using IntegrationDb.SuspiciousCases;
    using IntegrationDb.SuspiciousCases.Infrastructure;
    using IntegrationDb.SuspiciousCases.Views;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class CurrentStreetNameWithoutLinkedRoadSegments : SuspiciousCase
    {
        public int StreetNamePersistentLocalId { get; set; }

        public override Category Category => Category.RoadSegment;
    }

    public sealed class CurrentStreetNameWithoutLinkedRoadSegmentsConfiguration : IEntityTypeConfiguration<CurrentStreetNameWithoutLinkedRoadSegments>
    {
        public void Configure(EntityTypeBuilder<CurrentStreetNameWithoutLinkedRoadSegments> builder)
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
                            FROM {Schema.SuspiciousCases}.{ViewName}");

            builder.Property(x => x.PersistentLocalId).HasColumnName("persistent_local_id");
            builder.Property(x => x.StreetNamePersistentLocalId).HasColumnName("streetname_persistent_local_id");
            builder.Property(x => x.NisCode).HasColumnName("nis_code");
            builder.Property(x => x.Description).HasColumnName("description");
        }

        public const string ViewName = "view_current_street_name_without_linked_roadsegments";

        public const string Create = $@"
            CREATE MATERIALIZED VIEW IF NOT EXISTS {Schema.SuspiciousCases}.{ViewName} AS
            SELECT
                CAST(s.persistent_local_id as varchar) AS persistent_local_id,
		        s.persistent_local_id AS streetname_persistent_local_id,
		        s.nis_code AS nis_code,
		        s.name_dutch AS description
            FROM {SchemaLatestItems.StreetName} AS s
            WHERE NOT EXISTS (
                SELECT 1
                FROM {SchemaLatestItems.RoadSegment} rs
                WHERE rs.left_side_street_name_id = s.persistent_local_id
                AND rs.right_side_street_name_id = s.persistent_local_id
            )
            AND s.status = 1
            AND s.is_removed = false;
            ";
    }
}
