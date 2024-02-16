namespace Basisregisters.IntegrationDb.NationalRegistry.Matching
{
    using System;

    public static class LevenshteinDistanceCalculator
    {
        public static double CalculatePercentage(string a, string b)
        {
            var distance = Fastenshtein.Levenshtein.Distance(a.ToLower(), b.ToLower());
            var maxLength = Math.Max(a.Length, b.Length);

            var percentageDifference = (double)distance / maxLength * 100.0;
            return percentageDifference;
        }
    }
}
