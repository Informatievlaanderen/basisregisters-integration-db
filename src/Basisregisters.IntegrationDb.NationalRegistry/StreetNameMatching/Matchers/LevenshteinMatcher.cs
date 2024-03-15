namespace Basisregisters.IntegrationDb.NationalRegistry.StreetNameMatching.Matchers
{
    using System;
    using Fastenshtein;

    public class LevenshteinMatcher : IMatcher
    {
        private readonly int _maxLevenshteinDistanceInPercentage;

        public LevenshteinMatcher(int maxLevenshteinDistanceInPercentage = 10)
        {
            _maxLevenshteinDistanceInPercentage = maxLevenshteinDistanceInPercentage;
        }

        public bool Match(string? streetName, string search)
        {
            if (string.IsNullOrWhiteSpace(streetName))
                return false;

            var distance = Levenshtein.Distance(streetName.ToLower(), search.ToLower());
            var maxLength = Math.Max(streetName.Length, search.Length);

            var percentageDifference = (double)distance / maxLength * 100.0;

            return percentageDifference <= _maxLevenshteinDistanceInPercentage;
        }
    }
}
