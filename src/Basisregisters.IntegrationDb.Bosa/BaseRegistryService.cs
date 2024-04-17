namespace Basisregisters.IntegrationDb.Bosa
{
    using System;
    using Model;

    public class BaseRegistryService<T> where T : IHasVersionTimestamps
    {
        protected static bool ShouldUseNewVersion(T obj)
        {
            return obj.CrabVersionTimestamp is null ||
                   obj.VersionTimestamp >= new DateTime(2023, 11, 10);
        }
    }
}