namespace Basisregisters.IntegrationDb.DataIntegrity.Feeds;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class BuildingUnitFeedIntegrity
{
    public string PersistentLocalId { get; set; } = null!;
}

public sealed class BuildingUnitFeedIntegrityConfiguration : IEntityTypeConfiguration<BuildingUnitFeedIntegrity>
{
    public const string ViewName = "building_unit_feed_latest_integrity";

    public void Configure(EntityTypeBuilder<BuildingUnitFeedIntegrity> builder)
    {
        builder
            .ToView(ViewName, DataIntegrityContext.Schema)
            .HasNoKey()
            .ToSqlQuery(@$"SELECT persistent_local_id FROM {DataIntegrityContext.Schema}.{ViewName};");

        builder.Property(x => x.PersistentLocalId).HasColumnName("persistent_local_id");
    }
}

public sealed class BuildingUnitViewRepository : IFeedIntegrityRepository
{
    private readonly IDbContextFactory<DataIntegrityContext> _contextFactory;
    public const double GeometryToleranceInMeters = 0.1;

    public string RegisterName => "BuildingUnit";


    public BuildingUnitViewRepository(IDbContextFactory<DataIntegrityContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task RefreshViewAsync(CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        await context.Database.ExecuteSqlRawAsync(RefreshSql, cancellationToken);
    }

    public async Task<IEnumerable<string>> GetIntegrityErrorsAsync(CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        return await context.BuildingUnitFeedIntegrity
            .Select(x => x.PersistentLocalId)
            .ToListAsync(cancellationToken);
    }

    public static string SelectBuildingUnitLatestSql => @"
        SELECT
            building_unit_persistent_local_id as persistent_local_id,
            MD5(CONCAT(
	            building_unit_persistent_local_id,
	            building_persistent_local_id,
	            status,
	            ""function"",
	            geometry_method,
	            has_deviation,
	            is_removed,
	            version_as_string)) as hash,
            geometry
        FROM integration_building.building_unit_latest_items
";

    public static string SelectBuildingUnitFeedSql => @"
        SELECT
            persistent_local_id,
                MD5(CONCAT(persistent_local_id,
                building_persistent_local_id,
                status,
                ""function"",
                geometry_method,
                has_deviation,
                is_removed,
                version_id_as_string)) as hash,
            ""position"" as geometry
        FROM changefeed.buildingunits
";

    // We limit the result to 1000 rows to reduce the size of the materialized view.
    // If there are more than 1000, something is wrong anyway.
    public static string DropAndCreateBuildingUnitViewSql => global::System.FormattableString.Invariant($@"
        DROP MATERIALIZED VIEW IF EXISTS {DataIntegrityContext.Schema}.{BuildingUnitFeedIntegrityConfiguration.ViewName};

        CREATE MATERIALIZED VIEW {DataIntegrityContext.Schema}.{BuildingUnitFeedIntegrityConfiguration.ViewName} AS
        WITH latest AS (
            {SelectBuildingUnitLatestSql}
        ),
        feed AS (
            {SelectBuildingUnitFeedSql}
        ),
        comparison AS (
            SELECT
                COALESCE(latest.persistent_local_id, feed.persistent_local_id) as persistent_local_id,
                latest.hash as latest_hash,
                feed.hash as feed_hash,
                latest.geometry as latest_geometry,
                CASE
                    WHEN latest.geometry IS NULL OR feed.geometry IS NULL THEN NULL
                    WHEN ST_SRID(feed.geometry) = ST_SRID(latest.geometry) THEN feed.geometry
                    ELSE ST_Transform(feed.geometry, ST_SRID(latest.geometry))
                END as feed_geometry,
                latest.hash IS DISTINCT FROM feed.hash as hash_mismatch
            FROM latest
            FULL OUTER JOIN feed
                ON latest.persistent_local_id = feed.persistent_local_id
            WHERE feed.hash IS NOT NULL
        )
        SELECT
            persistent_local_id,
            latest_hash,
            feed_hash,
            hash_mismatch,
            CASE
                WHEN latest_geometry IS NULL AND feed_geometry IS NULL THEN false
                WHEN latest_geometry IS NULL OR feed_geometry IS NULL THEN true
                ELSE NOT ST_DWithin(latest_geometry, feed_geometry, {GeometryToleranceInMeters})
            END as geometry_mismatch,
            CASE
                WHEN latest_geometry IS NULL OR feed_geometry IS NULL THEN NULL
                ELSE ST_Distance(latest_geometry, feed_geometry)
            END as geometry_distance
        FROM comparison
        WHERE hash_mismatch
            OR CASE
                WHEN latest_geometry IS NULL AND feed_geometry IS NULL THEN false
                WHEN latest_geometry IS NULL OR feed_geometry IS NULL THEN true
                ELSE NOT ST_DWithin(latest_geometry, feed_geometry, {GeometryToleranceInMeters})
            END
        ORDER BY persistent_local_id
        LIMIT 1000;

        CREATE UNIQUE INDEX IF NOT EXISTS building_unit_feed_latest_integrity_persistent_local_id_idx
            ON {DataIntegrityContext.Schema}.{BuildingUnitFeedIntegrityConfiguration.ViewName} (persistent_local_id);
    ");

    public static string RefreshSql => $@"
        REFRESH MATERIALIZED VIEW CONCURRENTLY {DataIntegrityContext.Schema}.{BuildingUnitFeedIntegrityConfiguration.ViewName};
    ";
}
