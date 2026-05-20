namespace Basisregisters.IntegrationDb.DataIntegrity.Feeds;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class AddressFeedIntegrity
{
    public string PersistentLocalId { get; set; } = null!;
}

public sealed class AddressFeedIntegrityConfiguration : IEntityTypeConfiguration<AddressFeedIntegrity>
{
    public const string ViewName = "address_feed_latest_integrity";

    public void Configure(EntityTypeBuilder<AddressFeedIntegrity> builder)
    {
        builder
            .ToView(ViewName, DataIntegrityContext.Schema)
            .HasNoKey()
            .ToSqlQuery(@$"SELECT persistent_local_id FROM {DataIntegrityContext.Schema}.{ViewName};");

        builder.Property(x => x.PersistentLocalId).HasColumnName("persistent_local_id");
    }
}

public sealed class AddressViewRepository : IFeedIntegrityRepository
{
    private readonly IDbContextFactory<DataIntegrityContext> _contextFactory;
    public const double GeometryToleranceInMeters = 0.1;

    public string RegisterName => "Address";


    public AddressViewRepository(IDbContextFactory<DataIntegrityContext> contextFactory)
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

        return await context.AddressFeedIntegrity
            .Select(x => x.PersistentLocalId)
            .ToListAsync(cancellationToken);
    }

    public static string SelectAddressLatestSql => @"
        SELECT
	        persistent_local_id,
	        MD5(CONCAT(
	            persistent_local_id,
	            postal_code,
	            street_name_persistent_local_id,
	            status,
	            house_number,
	            box_number,
	            position_method,
	            position_specification,
	            CASE WHEN officially_assigned IS NULL THEN False ELSE officially_assigned END,
	            removed,
	            version_as_string)) as hash,
	        geometry
        FROM integration_address.address_latest_items_v2
";

    public static string SelectAddressFeedSql => @"
        SELECT
            persistent_local_id,
            MD5(CONCAT(
                persistent_local_id,
                postal_code,
                street_name_persistent_local_id,
                CASE WHEN status = 'Proposed' THEN 1
                    WHEN status = 'Current' THEN 2
                    WHEN status = 'Retired' THEN 3
                    WHEN status = 'Rejected' THEN 4
                END,
                house_number,
                box_number,
                CASE WHEN position_method = 'AppointedByAdministrator' THEN 1
                    WHEN position_method = 'DerivedFromObject' THEN 2
                    WHEN position_method = 'Interpolated' THEN 3
                END,
                CASE WHEN position_specification = 'Municipality' THEN 1
                    WHEN position_specification = 'Street' THEN 2
                    WHEN position_specification = 'Parcel' THEN 3
                    WHEN position_specification = 'Lot' THEN 4
                    WHEN position_specification = 'Stand' THEN 5
                    WHEN position_specification = 'Berth' THEN 6
                    WHEN position_specification = 'Building' THEN 7
                    WHEN position_specification = 'BuildingUnit' THEN 8
                    WHEN position_specification = 'Entry' THEN 9
                    WHEN position_specification = 'RoadSegment' THEN 11
                END,
                CASE WHEN officially_assigned IS NULL THEN False ELSE officially_assigned END,
                is_removed,
                version_id_as_string)) as hash,
            geometry
        FROM changefeed.addresses
";

    // We limit the result to 1000 rows to reduce the size of the materialized view.
    // If there are more than 1000, something is wrong anyway.
    public static string DropAndCreateAddressViewSql => global::System.FormattableString.Invariant($@"
        DROP MATERIALIZED VIEW IF EXISTS {DataIntegrityContext.Schema}.{AddressFeedIntegrityConfiguration.ViewName};

        CREATE MATERIALIZED VIEW {DataIntegrityContext.Schema}.{AddressFeedIntegrityConfiguration.ViewName} AS
        WITH latest AS (
            {SelectAddressLatestSql}
        ),
        feed AS (
            {SelectAddressFeedSql}
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

        CREATE UNIQUE INDEX IF NOT EXISTS address_feed_latest_integrity_persistent_local_id_idx
            ON {DataIntegrityContext.Schema}.{AddressFeedIntegrityConfiguration.ViewName} (persistent_local_id);
    ");

    public static string RefreshSql => $@"
        REFRESH MATERIALIZED VIEW CONCURRENTLY {DataIntegrityContext.Schema}.{AddressFeedIntegrityConfiguration.ViewName};
    ";
}
