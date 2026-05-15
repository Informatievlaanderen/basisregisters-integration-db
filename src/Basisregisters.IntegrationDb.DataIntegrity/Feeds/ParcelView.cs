namespace Basisregisters.IntegrationDb.DataIntegrity.Feeds;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class ParcelFeedIntegrity
{
    public string PersistentLocalId { get; set; } = null!;
}

public sealed class ParcelFeedIntegrityConfiguration : IEntityTypeConfiguration<ParcelFeedIntegrity>
{
    public const string ViewName = "parcel_feed_latest_integrity";

    public void Configure(EntityTypeBuilder<ParcelFeedIntegrity> builder)
    {
        builder
            .ToView(ViewName, DataIntegrityContext.Schema)
            .HasNoKey()
            .ToSqlQuery(@$"SELECT persistent_local_id FROM {DataIntegrityContext.Schema}.{ViewName};");

        builder.Property(x => x.PersistentLocalId).HasColumnName("persistent_local_id");
    }
}

public sealed class ParcelViewRepository : IFeedIntegrityRepository
{
    private readonly IDbContextFactory<DataIntegrityContext> _contextFactory;

    public string RegisterName => "Parcel";


    public ParcelViewRepository(IDbContextFactory<DataIntegrityContext> contextFactory)
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

        return await context.ParcelFeedIntegrity
            .Select(x => x.PersistentLocalId)
            .ToListAsync(cancellationToken);
    }

    public static string SelectParcelLatestSql => @"
        SELECT
            capakey as persistent_local_id,
            MD5(CONCAT(capakey,
                status,
                version_as_string)) as hash
	    FROM integration_parcel.parcel_latest_items_v2
";

    public static string SelectParcelSql => @"
        SELECT
            vbr_capakey as persistent_local_id,
            MD5(CONCAT(vbr_capakey,
                status,
                version_id_as_string)) as hash
	    FROM changefeed.parcels
";

    // We limit the result to 1000 rows to reduce the size of the materialized view.
    // If there are more than 1000, something is wrong anyway.
    public static string DropAndCreateParcelViewSql => $@"
        DROP MATERIALIZED VIEW IF EXISTS {DataIntegrityContext.Schema}.{ParcelFeedIntegrityConfiguration.ViewName};

        CREATE MATERIALIZED VIEW {DataIntegrityContext.Schema}.{ParcelFeedIntegrityConfiguration.ViewName} AS
        WITH latest AS (
            {SelectParcelLatestSql}
        ),
        feed AS (
            {SelectParcelSql}
        )
        SELECT
            COALESCE(latest.persistent_local_id, feed.persistent_local_id) as persistent_local_id,
            latest.hash as latest_hash,
            feed.hash as feed_hash
        FROM latest
        FULL OUTER JOIN feed
            ON latest.persistent_local_id = feed.persistent_local_id
        WHERE feed.hash IS NOT NULL
            AND latest.hash IS DISTINCT FROM feed.hash
        ORDER BY COALESCE(latest.persistent_local_id, feed.persistent_local_id)
        LIMIT 1000;

        CREATE UNIQUE INDEX IF NOT EXISTS parcel_feed_latest_integrity_persistent_local_id_idx
            ON {DataIntegrityContext.Schema}.{ParcelFeedIntegrityConfiguration.ViewName} (persistent_local_id);
    ";

    public static string RefreshSql => $@"
        REFRESH MATERIALIZED VIEW CONCURRENTLY {DataIntegrityContext.Schema}.{ParcelFeedIntegrityConfiguration.ViewName};
    ";
}
