namespace Basisregisters.IntegrationDb.DataIntegrity.Feeds;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class PostalFeedIntegrity
{
    public string PostalCode { get; set; } = null!;
}

public sealed class PostalFeedIntegrityConfiguration : IEntityTypeConfiguration<PostalFeedIntegrity>
{
    public const string ViewName = "postal_feed_latest_integrity";

    public void Configure(EntityTypeBuilder<PostalFeedIntegrity> builder)
    {
        builder
            .ToView(ViewName, DataIntegrityContext.Schema)
            .HasNoKey()
            .ToSqlQuery(@$"SELECT postal_code FROM {DataIntegrityContext.Schema}.postal_feed_latest_integrity;");

        builder.Property(x => x.PostalCode).HasColumnName("postal_code");
    }
}

public sealed class PostalViewRepository : IFeedIntegrityRepository
{
    private readonly IDbContextFactory<DataIntegrityContext> _contextFactory;

    public string RegisterName => "Postal";


    public PostalViewRepository(IDbContextFactory<DataIntegrityContext> contextFactory)
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

        return await context.PostalFeedIntegrity
            .Select(x => x.PostalCode)
            .ToListAsync(cancellationToken);
    }

    public static string SelectPostalLatestSql => @"
        SELECT
            postal_code,
            MD5(
	            CONCAT(
	            postal_code,
	            nis_code,
	            is_retired,
	            to_char(version_timestamp, 'YYYY-MM-DDTHH24:MI:SS'))) as hash
        FROM integration_postal.postal_latest_items
";

    public static string SelectPostalFeedSql => @"
        SELECT
            postal_code,
            MD5(
	            CONCAT(
	            postal_code,
	            nis_code,
	            CASE WHEN status = 'Realized' THEN False ELSE True END,
        	    to_char(version_id, 'YYYY-MM-DDTHH24:MI:SS'))) as hash
	    FROM changefeed.postal_information
	    WHERE is_removed = false
";

    // We limit the result to 1000 rows to reduce the size of the materialized view.
    // If there are more than 1000, something is wrong anyway.
    public static string DropAndCreatePostalViewSql => $@"
        DROP MATERIALIZED VIEW IF EXISTS {DataIntegrityContext.Schema}.{PostalFeedIntegrityConfiguration.ViewName};

        CREATE MATERIALIZED VIEW {DataIntegrityContext.Schema}.{PostalFeedIntegrityConfiguration.ViewName} AS
        WITH latest AS (
            {SelectPostalLatestSql}
        ),
        feed AS (
            {SelectPostalFeedSql}
        )
        SELECT
            COALESCE(latest.postal_code, feed.postal_code) as postal_code,
            latest.hash as latest_hash,
            feed.hash as feed_hash
        FROM latest
        FULL OUTER JOIN feed
            ON latest.postal_code = feed.postal_code
        WHERE latest.hash IS DISTINCT FROM feed.hash
        ORDER BY COALESCE(latest.postal_code, feed.postal_code)
        LIMIT 1000;

        CREATE UNIQUE INDEX IF NOT EXISTS postal_feed_latest_integrity_postal_code_idx
            ON {DataIntegrityContext.Schema}.{PostalFeedIntegrityConfiguration.ViewName} (postal_code);
    ";

    public static string RefreshSql => $@"
        REFRESH MATERIALIZED VIEW CONCURRENTLY {DataIntegrityContext.Schema}.{PostalFeedIntegrityConfiguration.ViewName};
    ";
}
