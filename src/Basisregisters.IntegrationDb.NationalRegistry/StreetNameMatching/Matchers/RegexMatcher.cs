namespace Basisregisters.IntegrationDb.NationalRegistry.StreetNameMatching.Matchers
{
    using System.Text.RegularExpressions;

    public class RegexMatcher : IMatcher
    {
        public bool Match(string? streetName, string search)
        {
            if (string.IsNullOrWhiteSpace(streetName))
                return false;

            if (!search.Contains('.'))
                return false;

            var pattern = Regex.Escape(search).Replace("\\.", "[a-zA-Z .]+");

            var regex = new Regex(pattern, RegexOptions.IgnoreCase);

            return regex.IsMatch(streetName);
        }
    }
}
