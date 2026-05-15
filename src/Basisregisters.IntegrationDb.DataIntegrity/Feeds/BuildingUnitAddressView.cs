namespace Basisregisters.IntegrationDb.DataIntegrity.Feeds;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class BuildingUnitAddressFeedIntegrity
{
    public string PersistentLocalId { get; set; } = null!;
}

public sealed class BuildingUnitAddressFeedIntegrityConfiguration : IEntityTypeConfiguration<BuildingUnitAddressFeedIntegrity>
{
    public const string ViewName = "building_unit_addresses_feed_latest_integrity";

    public void Configure(EntityTypeBuilder<BuildingUnitAddressFeedIntegrity> builder)
    {
        builder
            .ToView(ViewName, DataIntegrityContext.Schema)
            .HasNoKey()
            .ToSqlQuery(@$"SELECT persistent_local_id FROM {DataIntegrityContext.Schema}.{ViewName};");

        builder.Property(x => x.PersistentLocalId).HasColumnName("persistent_local_id");
    }
}

public sealed class BuildingUnitAddressViewRepository : IFeedIntegrityRepository
{
    private readonly IDbContextFactory<DataIntegrityContext> _contextFactory;

    public string RegisterName => "BuildingUnitAddress";


    public BuildingUnitAddressViewRepository(IDbContextFactory<DataIntegrityContext> contextFactory)
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

        return await context.BuildingUnitAddressFeedIntegrity
            .Select(x => x.PersistentLocalId)
            .ToListAsync(cancellationToken);
    }

    public static string SelectBuildingUnitAddressLatestSql => @"
        SELECT
            building_unit_persistent_local_id as persistent_local_id,
            MD5(
                STRING_AGG(
                    address_persistent_local_id::text,
                    '|' ORDER BY address_persistent_local_id)) as hash
        FROM (
            SELECT DISTINCT
                building_unit_persistent_local_id,
                address_persistent_local_id
	        FROM integration_building.building_unit_addresses
            WHERE building_unit_persistent_local_id IS NOT NULL
                AND address_persistent_local_id IS NOT NULL
        ) latest
        GROUP BY building_unit_persistent_local_id
";

    public static string SelectBuildingUnitAddressFeedSql => @"
        SELECT
            buildingunit_persistent_local_id as persistent_local_id,
            MD5(
                STRING_AGG(
                    address_persistent_local_id::text,
                    '|' ORDER BY address_persistent_local_id)) as hash
        FROM (
            SELECT DISTINCT
                buildingunit_persistent_local_id,
                address_persistent_local_id
	        FROM changefeed.buildingunit_addresses
            WHERE buildingunit_persistent_local_id IS NOT NULL
                AND address_persistent_local_id IS NOT NULL
        ) feed
        GROUP BY buildingunit_persistent_local_id
";

    // We limit the result to 1000 rows to reduce the size of the materialized view.
    // If there are more than 1000, something is wrong anyway.
    public static string DropAndCreateBuildingUnitAddressViewSql => global::System.FormattableString.Invariant($@"
        DROP MATERIALIZED VIEW IF EXISTS {DataIntegrityContext.Schema}.{BuildingUnitAddressFeedIntegrityConfiguration.ViewName};

        CREATE MATERIALIZED VIEW {DataIntegrityContext.Schema}.{BuildingUnitAddressFeedIntegrityConfiguration.ViewName} AS
        WITH latest AS (
            {SelectBuildingUnitAddressLatestSql}
        ),
        feed AS (
            {SelectBuildingUnitAddressFeedSql}
        ),
        comparison AS (
            SELECT
                COALESCE(latest.persistent_local_id, feed.persistent_local_id) as persistent_local_id,
                latest.hash as latest_hash,
                feed.hash as feed_hash,
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
            hash_mismatch
        FROM comparison
        WHERE hash_mismatch
        ORDER BY persistent_local_id
        LIMIT 1000;

        CREATE UNIQUE INDEX IF NOT EXISTS building_unit_addresses_feed_latest_integrity_persistent_local_id_idx
            ON {DataIntegrityContext.Schema}.{BuildingUnitAddressFeedIntegrityConfiguration.ViewName} (persistent_local_id);
    ");

    public static string RefreshSql => $@"
        REFRESH MATERIALIZED VIEW CONCURRENTLY {DataIntegrityContext.Schema}.{BuildingUnitAddressFeedIntegrityConfiguration.ViewName};
    ";
}
