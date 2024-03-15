namespace Basisregisters.IntegrationDb.NationalRegistry.Extensions
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    public static class ConcurrentBagExtensions
    {
        public static void AddRange<T>(this ConcurrentBag<T> bag, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                bag.Add(item);
            }
        }
    }
}
