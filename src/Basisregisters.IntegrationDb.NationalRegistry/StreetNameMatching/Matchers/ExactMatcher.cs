namespace Basisregisters.IntegrationDb.NationalRegistry.StreetNameMatching.Matchers
{
    using System;

    public class ExactMatcher : IMatcher
    {
        public bool Match(string? streetName, string search)
        {
            return string.Equals(streetName, search, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
