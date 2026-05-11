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
            .ToSqlQuery(@$"SELECT nis_code FROM {DataIntegrityContext.Schema}.municipality_feed_latest_integrity;");

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

    public async Task<IEnumerable<string>> GetIntegrityErrorsAsync(CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        return await context.MunicipalityFeedIntegrity
            .Select(x => x.NisCode)
            .ToListAsync(cancellationToken);
    }

    public static string SelectMunicipalityLatestSql => @"
        SELECT
            nis_code,
            MD5(Concat(
                UPPER(nis_code),
                status,
                UPPER(name_dutch),
                UPPER(name_french),
                UPPER(name_german),
                UPPER(name_english),
                CASE WHEN official_language_dutch is NULL THEN FALSE ELSE official_language_dutch END,
                CASE WHEN official_language_french is NULL THEN FALSE ELSE official_language_french END,
                CASE WHEN official_language_german is NULL THEN FALSE ELSE official_language_german END,
                CASE WHEN official_language_english is NULL THEN FALSE ELSE official_language_english END,
                CASE WHEN facility_language_dutch is NULL THEN FALSE ELSE facility_language_dutch END,
                CASE WHEN facility_language_french is NULL THEN FALSE ELSE facility_language_french END,
                CASE WHEN facility_language_german is NULL THEN FALSE ELSE facility_language_german END,
                CASE WHEN facility_language_english is NULL THEN FALSE ELSE facility_language_english END,
                is_removed,
                UPPER(version_as_string))) as hash
        FROM integration_municipality.municipality_latest_items
";

    public static string SelectMunicipalityFeedSql => @"
        SELECT
            nis_code,
            MD5(Concat(
                UPPER(nis_code),
                CASE WHEN UPPER(status) = 'PROPOSED' THEN 2 WHEN UPPER(status) = 'CURRENT' THEN 0 WHEN UPPER(status) = 'RETIRED' THEN 1 END,
                UPPER(name_dutch),
                UPPER(name_french),
                UPPER(name_german),
                UPPER(name_english),
                CASE WHEN official_language_dutch is NULL THEN FALSE ELSE official_language_dutch END,
                CASE WHEN official_language_french is NULL THEN FALSE ELSE official_language_french END,
                CASE WHEN official_language_german is NULL THEN FALSE ELSE official_language_german END,
                CASE WHEN official_language_english is NULL THEN FALSE ELSE official_language_english END,
                CASE WHEN facility_language_dutch is NULL THEN FALSE ELSE facility_language_dutch END,
                CASE WHEN facility_language_french is NULL THEN FALSE ELSE facility_language_french END,
                CASE WHEN facility_language_german is NULL THEN FALSE ELSE facility_language_german END,
                CASE WHEN facility_language_english is NULL THEN FALSE ELSE facility_language_english END,
                is_removed,
                UPPER(version_id_as_string))) as hash
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
        ORDER BY COALESCE(latest.nis_code, feed.nis_code)
        LIMIT 1000;

        CREATE UNIQUE INDEX IF NOT EXISTS municipality_feed_latest_integrity_nis_code_idx
            ON {DataIntegrityContext.Schema}.{MunicipalityFeedIntegrityConfiguration.ViewName} (nis_code);
    ";

    public static string RefreshSql => $@"
        REFRESH MATERIALIZED VIEW CONCURRENTLY {DataIntegrityContext.Schema}.{MunicipalityFeedIntegrityConfiguration.ViewName};
    ";
}
