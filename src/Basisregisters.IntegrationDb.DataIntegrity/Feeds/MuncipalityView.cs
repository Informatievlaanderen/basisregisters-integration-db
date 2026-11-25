namespace Basisregisters.IntegrationDb.DataIntegrity.Feeds;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class MunicipalityFeedIntegrity
{
    public string NisCode { get; private set; } = null!;
}

public sealed class MunicipalityFeedIntegrityConfiguration : IEntityTypeConfiguration<MunicipalityFeedIntegrity>
{
    public const string ViewName = "municipality_feed_latest_integrity";

    public void Configure(EntityTypeBuilder<MunicipalityFeedIntegrity> builder)
    {
        builder
            .ToView(ViewName, DataIntegrityContext.Schema)
            .HasNoKey()
            .ToSqlQuery(@$"SELECT nis_code FROM {DataIntegrityContext.Schema}.municipality_feed_latest_integrity");

        builder.Property(x => x.NisCode).HasColumnName("nis_code");
    }
}

public sealed class MunicipalityViewRepository : IFeedIntegrityRepository
{
    private readonly IDbContextFactory<DataIntegrityContext> _contextFactory;

    public string RegisterName => "Municipality";

    public MunicipalityViewRepository(IDbContextFactory<DataIntegrityContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task RefreshViewAsync(CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        await context.Database.ExecuteSqlRawAsync(RefreshSql, cancellationToken);
    }

    public async Task<IReadOnlyList<string>> GetIntegrityErrorsAsync(CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        return await context.MunicipalityFeedIntegrity
            .Select(x => x.NisCode)
            .ToListAsync(cancellationToken);
    }

    private static readonly string[] LanguageColumns =
    [
        "COALESCE(official_language_dutch, FALSE)",
        "COALESCE(official_language_french, FALSE)",
        "COALESCE(official_language_german, FALSE)",
        "COALESCE(official_language_english, FALSE)",
        "COALESCE(facility_language_dutch, FALSE)",
        "COALESCE(facility_language_french, FALSE)",
        "COALESCE(facility_language_german, FALSE)",
        "COALESCE(facility_language_english, FALSE)",
    ];

    // Names are compared case-sensitively, so a casing error (Sint-niklaas vs Sint-Niklaas) is reported.
    public static string SelectMunicipalityLatestSql => $@"
        SELECT
            nis_code,
            {FeedSql.Hash([
                "UPPER(nis_code)",
                "status",
                "name_dutch",
                "name_french",
                "name_german",
                "name_english",
                ..LanguageColumns,
                "is_removed",
                "UPPER(version_as_string)"])} as hash,
            is_removed
        FROM integration_municipality.municipality_latest_items
";

    public static string SelectMunicipalityFeedSql => $@"
        SELECT
            nis_code,
            {FeedSql.Hash([
                "UPPER(nis_code)",
                "CASE WHEN UPPER(status) = 'PROPOSED' THEN 2 WHEN UPPER(status) = 'CURRENT' THEN 0 WHEN UPPER(status) = 'RETIRED' THEN 1 END",
                "name_dutch",
                "name_french",
                "name_german",
                "name_english",
                ..LanguageColumns,
                "is_removed",
                "UPPER(version_id_as_string)"])} as hash
        FROM changefeed.municipalities
";

    // We limit the result to 1000 rows to reduce the size of the materialized view.
    // If there are more than 1000, something is wrong anyway.
    public static string DropAndCreateMunicipalityViewSql => $@"
        DROP MATERIALIZED VIEW IF EXISTS {DataIntegrityContext.Schema}.{MunicipalityFeedIntegrityConfiguration.ViewName};

        CREATE MATERIALIZED VIEW {DataIntegrityContext.Schema}.{MunicipalityFeedIntegrityConfiguration.ViewName} AS
        WITH latest AS (
            {SelectMunicipalityLatestSql}
        ),
        feed AS (
            {SelectMunicipalityFeedSql}
        )
        SELECT
            COALESCE(latest.nis_code, feed.nis_code) as nis_code,
            latest.hash as latest_hash,
            feed.hash as feed_hash
        FROM latest
        FULL OUTER JOIN feed
            ON latest.nis_code = feed.nis_code
        WHERE latest.hash IS DISTINCT FROM feed.hash
            AND {FeedSql.SkipRemovedMissingFromFeed}
        ORDER BY COALESCE(latest.nis_code, feed.nis_code)
        LIMIT {FeedSql.MaxRows};

        CREATE UNIQUE INDEX IF NOT EXISTS municipality_feed_latest_integrity_nis_code_idx
            ON {DataIntegrityContext.Schema}.{MunicipalityFeedIntegrityConfiguration.ViewName} (nis_code);
    ";

    public static string RefreshSql => $@"
        REFRESH MATERIALIZED VIEW CONCURRENTLY {DataIntegrityContext.Schema}.{MunicipalityFeedIntegrityConfiguration.ViewName};
    ";
}
