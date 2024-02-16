namespace Basisregisters.IntegrationDb.NationalRegistry.Matching
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Repositories;

    public class StreetNameMatcher
    {
        private readonly IEnumerable<StreetName> _streetNames;

        public StreetNameMatcher(IEnumerable<StreetName> streetNames)
        {
            _streetNames = streetNames;
        }

        public int? MatchStreetName(string search)
        {
            var streetNameMatch = Match(search);
            if (streetNameMatch.HasValue)
            {
                return streetNameMatch;
            }

            var searchByAbbreviation = ReplaceAbbreviation(search);
            streetNameMatch = Match(searchByAbbreviation);
            if (streetNameMatch.HasValue)
            {
                return streetNameMatch;
            }

            // streetNameMatch = IsHomonymMatch(search);

            return null;
        }

        private int? Match(string search)
        {
            var match = _streetNames.FirstOrDefault(x =>
                    string.Equals(x.NameDutch, search, StringComparison.InvariantCultureIgnoreCase) ||
                    string.Equals(x.NameFrench, search, StringComparison.InvariantCultureIgnoreCase) ||
                    string.Equals(x.NameGerman, search, StringComparison.InvariantCultureIgnoreCase) ||
                    string.Equals(x.NameEnglish, search, StringComparison.InvariantCultureIgnoreCase))
                ?.StreetNamePersistentLocalId;

            if (match.HasValue)
            {
                return match;
            }

            var searchWithoutDiacritics = RemoveDiacritics(search);

            return _streetNames.FirstOrDefault(x =>
                    string.Equals(x.NameDutch, searchWithoutDiacritics, StringComparison.InvariantCultureIgnoreCase) ||
                    string.Equals(x.NameFrench, searchWithoutDiacritics, StringComparison.InvariantCultureIgnoreCase) ||
                    string.Equals(x.NameGerman, searchWithoutDiacritics, StringComparison.InvariantCultureIgnoreCase) ||
                    string.Equals(x.NameEnglish, searchWithoutDiacritics, StringComparison.InvariantCultureIgnoreCase))
                ?.StreetNamePersistentLocalId;
        }

        private string ReplaceAbbreviation(string search)
        {
            var replaced = search.ToLowerInvariant();

            if (replaced.Contains("onze lieve vrouw"))
                return replaced.Replace("onze lieve vrouw", "O.L. Vrouw");

            if (replaced.Contains("o.l.v."))
                return replaced.Replace("o.l.v.", "Onze Lieve Vrouw");

            if (replaced.Contains("onze-lieve-"))
                return replaced.Replace("onze-lieve-", "O.L. ");

            if (replaced.Contains("sint"))
                return replaced.Replace("sint", "st.");

            if (replaced.Contains("st."))
                return replaced.Replace("st.", "sint");

            if (replaced.Contains("dr."))
                return replaced.Replace("dr.", "dokter");

            if (replaced.Contains("dokter"))
                return replaced.Replace("dokter", "dr.");

            if (replaced.Contains("stwg."))
                return replaced.Replace("stwg.", "steenweg");

            if (replaced.Contains("stwg"))
                return replaced.Replace("stwg", "steenweg");

            if (replaced.Contains("stw."))
                return replaced.Replace("stw.", "steenweg");

            if (replaced.Contains("stw"))
                return replaced.Replace("stw", "steenweg");

            if (replaced.Contains("burg."))
                return replaced.Replace("burg.", "Burgemeester");

            if (replaced.Contains("burgemeester"))
                return replaced.Replace("burgemeester", "burg.");

            if (replaced.EndsWith("str"))
                return replaced.Replace("str", "straat");

            if (replaced.EndsWith("str."))
                return replaced.Replace("str.", "straat");

            if (replaced.StartsWith("heilige"))
                return replaced.Replace("heilige", "h");

            if (replaced.StartsWith("heilig"))
                return replaced.Replace("heilig", "h");

            if (replaced.StartsWith("k."))
                return replaced.Replace("k.", "koning");

            return replaced;
        }

        private static string RemoveDiacritics(string search)
        {
            var stringBuilder = new StringBuilder(search.Length);
            var normalizedString = search.Normalize(NormalizationForm.FormD);

            foreach (var character in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(character);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                    stringBuilder.Append(character);
            }

            var resultString = stringBuilder.ToString();

            return resultString.Normalize(NormalizationForm.FormC);
        }
    }
}
