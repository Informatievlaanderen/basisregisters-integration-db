namespace Basisregisters.IntegrationDb.NationalRegistry.Extensions
{
    using System.Collections.Generic;

    public static class KeyValuePairExtensions
    {
        public static bool HasValue<T, U>(this KeyValuePair<T, U> keyValuePair)
        {
            return !keyValuePair.Equals(default(KeyValuePair<T, U>));
        }
    }
}
