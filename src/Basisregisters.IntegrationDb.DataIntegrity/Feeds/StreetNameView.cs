namespace Basisregisters.IntegrationDb.DataIntegrity.Feeds;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class StreetNameFeedIntegrity
{
    public string PersistentLocalId { get; set; } = null!;
}

public sealed class StreetNameFeedIntegrityConfiguration : IEntityTypeConfiguration<StreetNameFeedIntegrity>
{
    public const string ViewName = "streetname_feed_latest_integrity";

    public void Configure(EntityTypeBuilder<StreetNameFeedIntegrity> builder)
    {
        builder
            .ToView(ViewName, DataIntegrityContext.Schema)
            .HasNoKey()
            .ToSqlQuery(@$"SELECT persistent_local_id FROM {DataIntegrityContext.Schema}.{ViewName};");

        builder.Property(x => x.PersistentLocalId).HasColumnName("persistent_local_id");
    }
}

public sealed class StreetNameViewRepository : IFeedIntegrityRepository
{
    private readonly IDbContextFactory<DataIntegrityContext> _contextFactory;

    public string RegisterName => "StreetName";


    public StreetNameViewRepository(IDbContextFactory<DataIntegrityContext> contextFactory)
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

        return await context.StreetNameFeedIntegrity
            .Select(x => x.PersistentLocalId)
            .ToListAsync(cancellationToken);
    }

    public static string SelectStreetNameLatestSql => @"
        SELECT
	        persistent_local_id,
	        MD5(CONCAT(
	        persistent_local_id,
	        status,
	        nis_code,
	        CASE WHEN name_dutch is null THEN '' ELSE name_dutch END,
	        CASE WHEN name_french is null THEN '' ELSE name_french END,
	        CASE WHEN name_german is null THEN '' ELSE name_german END,
	        CASE WHEN name_english is null THEN '' ELSE name_english END,
	        CASE WHEN homonym_addition_dutch is null THEN '' ELSE homonym_addition_dutch END,
	        CASE WHEN homonym_addition_french is null THEN '' ELSE homonym_addition_french END,
	        CASE WHEN homonym_addition_german is null THEN '' ELSE homonym_addition_german END,
	        CASE WHEN homonym_addition_english is null THEN '' ELSE homonym_addition_english END,
	        is_removed,
	        version_as_string)) as hash
        FROM integration_streetname.streetname_latest_items
";

    public static string SelectStreetNameFeedSql => @"
        SELECT
	        persistent_local_id,
	        MD5(CONCAT(
	        persistent_local_id,
	        CASE WHEN status = 'Proposed' THEN 0
		         WHEN status = 'Current' THEN 1
	             WHEN status = 'Retired' THEN 2
	             WHEN status = 'Rejected' THEN 3
	        ELSE -1
	        END,
	        nis_code,
	        CASE WHEN name_dutch is null THEN '' ELSE name_dutch END,
	        CASE WHEN name_french is null THEN '' ELSE name_french END,
	        CASE WHEN name_german is null THEN '' ELSE name_german END,
	        CASE WHEN name_english is null THEN '' ELSE name_english END,
	        CASE WHEN homonym_addition_dutch is null THEN '' ELSE homonym_addition_dutch END,
	        CASE WHEN homonym_addition_french is null THEN '' ELSE homonym_addition_french END,
	        CASE WHEN homonym_addition_german is null THEN '' ELSE homonym_addition_german END,
	        CASE WHEN homonym_addition_english is null THEN '' ELSE homonym_addition_english END,
	        is_removed,
	        version_id_as_string)) as hash
        FROM changefeed.streetnames
";

    // We limit the result to 1000 rows to reduce the size of the materialized view.
    // If there are more than 1000, something is wrong anyway.
    public static string DropAndCreateStreetNameViewSql => $@"
        DROP MATERIALIZED VIEW IF EXISTS {DataIntegrityContext.Schema}.{StreetNameFeedIntegrityConfiguration.ViewName};

        CREATE MATERIALIZED VIEW {DataIntegrityContext.Schema}.{StreetNameFeedIntegrityConfiguration.ViewName} AS
        WITH latest AS (
            {SelectStreetNameLatestSql}
        ),
        feed AS (
            {SelectStreetNameFeedSql}
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

        CREATE UNIQUE INDEX IF NOT EXISTS streetname_feed_latest_integrity_persistent_local_id_idx
            ON {DataIntegrityContext.Schema}.{StreetNameFeedIntegrityConfiguration.ViewName} (persistent_local_id);
    ";

    public static string RefreshSql => $@"
        REFRESH MATERIALIZED VIEW CONCURRENTLY {DataIntegrityContext.Schema}.{StreetNameFeedIntegrityConfiguration.ViewName};
    ";
}
