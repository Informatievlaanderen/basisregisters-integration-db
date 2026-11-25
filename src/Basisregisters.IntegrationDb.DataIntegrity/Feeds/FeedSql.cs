namespace Basisregisters.IntegrationDb.DataIntegrity.Feeds;

using System.Linq;

internal static class FeedSql
{
    /// <summary>
    /// Builds an MD5 hash over the given column expressions.
    /// Every field gets a fixed position, separated by the ASCII unit separator (chr(31)),
    /// so values cannot shift between fields (e.g. house number 1 + box 23 vs house number 12 + box 3,
    /// or a name in the wrong language column). NULL is still treated as an empty string.
    /// </summary>
    public static string Hash(params string[] expressions)
        => $"MD5(CONCAT_WS(chr(31), {string.Join(", ", expressions.Select(e => $"COALESCE(({e})::text, '')"))}))";

    /// <summary>
    /// Removed objects are not published in the feed, so an object that is missing from the feed
    /// is only reported when it is not removed in the projection. Requires latest.is_removed.
    /// </summary>
    public const string SkipRemovedMissingFromFeed = "NOT (feed.hash IS NULL AND COALESCE(latest.is_removed, false))";

    /// <summary>
    /// Maximum number of rows in an integrity view. Keeps the refresh cheap: expensive output columns
    /// (e.g. geometry distances) are only computed for the returned rows.
    /// When a view returns this many rows, the real number of mismatches is higher.
    /// </summary>
    public const int MaxRows = 1000;
}
