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
            var streetNameMatch = IsExactMatch(search);
            if (streetNameMatch.HasValue)
            {
                return streetNameMatch;
            }

            // streetNameMatch = IsHomonymMatch(search);

            return null;
        }

        private int? IsExactMatch(string search)
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

        private static string RemoveDiacritics(string input)
        {
            var stringBuilder = new StringBuilder(input.Length);
            var normalizedString = input.Normalize(NormalizationForm.FormD);

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
