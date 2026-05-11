namespace Basisregisters.IntegrationDb.DataIntegrity.Feeds;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class PostalNameFeedIntegrity
{
    public string PostalCode { get; set; } = null!;
}

public sealed class PostalNameFeedIntegrityConfiguration : IEntityTypeConfiguration<PostalNameFeedIntegrity>
{
    public const string ViewName = "postal_name_feed_latest_integrity";

    public void Configure(EntityTypeBuilder<PostalNameFeedIntegrity> builder)
    {
        builder
            .ToView(ViewName, DataIntegrityContext.Schema)
            .HasNoKey()
            .ToSqlQuery(@$"SELECT postal_code FROM {DataIntegrityContext.Schema}.{ViewName};");

        builder.Property(x => x.PostalCode).HasColumnName("postal_code");
    }
}

public sealed class PostalNameViewRepository : IFeedIntegrityRepository
{
    private readonly IDbContextFactory<DataIntegrityContext> _contextFactory;

    public string RegisterName => "PostalName";


    public PostalNameViewRepository(IDbContextFactory<DataIntegrityContext> contextFactory)
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

        return await context.PostalNameFeedIntegrity
            .Select(x => x.PostalCode)
            .ToListAsync(cancellationToken);
    }

    public static string SelectPostalLatestSql => @"
        SELECT
            postal_code,
            MD5(STRING_AGG(
                CONCAT_WS('|',
                    COALESCE(language::text, ''),
                    COALESCE(name, '')),
                '||'
                ORDER BY language, name)) as hash
	    FROM integration_postal.postal_information_name
	    WHERE postal_code is not NULL
	    GROUP BY postal_code
";

    public static string SelectPostalFeedSql => @"
        SELECT
            pn.postal_code,
            MD5(STRING_AGG(
                CONCAT_WS('|',
                    COALESCE((language - 1)::text, ''),
                    COALESCE(name, '')),
                '||'
                ORDER BY language - 1, name)) as hash
	    FROM changefeed.postal_information_name pn
	    INNER JOIN changefeed.postal_information p ON p.postal_code = pn.postal_code AND p.is_removed = false
	    GROUP BY pn.postal_code
";

    // We limit the result to 1000 rows to reduce the size of the materialized view.
    // If there are more than 1000, something is wrong anyway.
    public static string DropAndCreatePostalNameViewSql => $@"
        DROP MATERIALIZED VIEW IF EXISTS {DataIntegrityContext.Schema}.{PostalNameFeedIntegrityConfiguration.ViewName};

        CREATE MATERIALIZED VIEW {DataIntegrityContext.Schema}.{PostalNameFeedIntegrityConfiguration.ViewName} AS
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

        CREATE UNIQUE INDEX IF NOT EXISTS postal_name_feed_latest_integrity_postal_code_idx
            ON {DataIntegrityContext.Schema}.{PostalNameFeedIntegrityConfiguration.ViewName} (postal_code);
    ";

    public static string RefreshSql => $@"
        REFRESH MATERIALIZED VIEW CONCURRENTLY {DataIntegrityContext.Schema}.{PostalNameFeedIntegrityConfiguration.ViewName};
    ";
}
