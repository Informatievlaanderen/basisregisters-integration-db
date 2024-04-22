namespace Basisregisters.IntegrationDb.Bosa
{
    using System;
    using Extensions;
    using Model;

    public class BaseRegistryService<T> where T : IHasVersionTimestamps
    {
        private static readonly DateTime NewVersionMigration = new (2023, 11, 10);

        protected static string GetVersionAsString(T obj)
        {
            return GetVersionAsString(obj.CrabVersionTimestamp, obj.VersionTimestamp);
        }

        protected static string GetVersionAsString(string? crabVersionTimestamp, DateTimeOffset versionTimestamp)
        {
            return crabVersionTimestamp is null ||
                   versionTimestamp >= NewVersionMigration
                ? versionTimestamp.ToBelgianString()
                : crabVersionTimestamp;
        }

        protected static string GetVersionAsString(DateTimeOffset value)
        {
            return value >= NewVersionMigration
                ? value.ToBelgianString()
                : value.ToString("yyyy-MM-ddTHH:mm:ss");
        }
    }
}
