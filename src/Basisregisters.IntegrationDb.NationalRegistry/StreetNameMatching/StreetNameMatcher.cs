namespace Basisregisters.IntegrationDb.NationalRegistry.StreetNameMatching
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Extensions;
    using Fastenshtein;
    using Repositories;
    using Sanitizers;

    public class StreetNameMatcher
    {
        private readonly IEnumerable<StreetName> _streetNames;
        private readonly int _maxLevenshteinDistanceInPercentage;

        private readonly IList<SanitizerBase> _sanitizers;

        public StreetNameMatcher(
            IEnumerable<StreetName> streetNames,
            int maxLevenshteinDistanceInPercentage = 10)
        {
            _streetNames = streetNames.ToList();
            _maxLevenshteinDistanceInPercentage = maxLevenshteinDistanceInPercentage;

            foreach (var streetName in _streetNames)
            {
                streetName.NameDutch = streetName.NameDutch?.RemoveDiacritics().RemoveQuotations().RemoveSpecialCharacters() ?? string.Empty;
                streetName.NameFrench = streetName.NameFrench?.RemoveDiacritics().RemoveQuotations().RemoveSpecialCharacters() ?? string.Empty;
                streetName.NameGerman = streetName.NameGerman?.RemoveDiacritics().RemoveQuotations().RemoveSpecialCharacters() ?? string.Empty;
                streetName.NameEnglish = streetName.NameEnglish?.RemoveDiacritics().RemoveQuotations().RemoveSpecialCharacters() ?? string.Empty;
            }

            _sanitizers = new List<SanitizerBase>
            {
                new AsIsSanitizer(), // We need to have a sanitizer to perform an exact match.
                new AbbreviationSanitizer(),
                new RemoveRoundBracketsSuffixSanitizer(),
                new UseRoundBracketsSuffixAsPrefixSanitizer(),
                new ReplaceCharsSanitizer(),
                new SpacingSanitizer(new SanitizerBase[] { new RemoveRoundBracketsSuffixSanitizer(), new AbbreviationSanitizer() }),
                new TrimEndSanitizer(new SanitizerBase[] { new AbbreviationSanitizer(), new UseRoundBracketsSuffixAsPrefixSanitizer() }),
            };
        }

        public IEnumerable<StreetName> MatchStreetName(string nisCode, string search)
        {
            var searchValue = search;
            if (HardCodedStreetNames.StreetNames.Any(x => x.NisCode == nisCode && x.SearchValue == search))
            {
                searchValue =
                    HardCodedStreetNames.StreetNames.Single(x => x.NisCode == nisCode && x.SearchValue == search).GrarValue;
            }

            foreach (var sanitizer in _sanitizers)
            {
                var streetNameMatch = Match(searchValue, sanitizer, MatchExact);
                if (streetNameMatch.Any())
                {
                    return streetNameMatch;
                }

                streetNameMatch = Match(searchValue, sanitizer, MatchByLevenshteinDistance);
                if (streetNameMatch.Any())
                {
                    return streetNameMatch;
                }

                streetNameMatch = Match(searchValue, sanitizer, MatchByRegex);
                if (streetNameMatch.Any())
                {
                    return streetNameMatch;
                }
            }

            return new List<StreetName>();
        }

        private bool MatchExact(string? streetName, string search)
            => string.Equals(streetName, search, StringComparison.InvariantCultureIgnoreCase);

        private bool MatchByLevenshteinDistance(string? streetName, string search)
        {
            if (string.IsNullOrWhiteSpace(streetName))
                return false;

            var distance = Levenshtein.Distance(streetName.ToLower(), search.ToLower());
            var maxLength = Math.Max(streetName.Length, search.Length);

            var percentageDifference = (double)distance / maxLength * 100.0;

            return percentageDifference <= _maxLevenshteinDistanceInPercentage;
        }

        private bool MatchByRegex(string? streetName, string search)
        {
            if (string.IsNullOrWhiteSpace(streetName))
                return false;

            if (!search.Contains('.'))
                return false;

            var pattern = Regex.Escape(search).Replace("\\.", "[a-zA-Z .]+");

            var regex = new Regex(pattern, RegexOptions.IgnoreCase);

            return regex.IsMatch(streetName);
        }

        private List<StreetName> Match(string search, SanitizerBase sanitizer, Func<string, string, bool> comparer)
        {
            var matches = _streetNames.Where(x => Match(x, search, sanitizer, comparer)).ToList();

            if (matches.Any())
            {
                return matches;
            }

            var sanitizedSearch = search.RemoveDiacritics().RemoveQuotations().RemoveSpecialCharacters();

            matches = _streetNames.Where(x => Match(x, sanitizedSearch, sanitizer, comparer)).ToList();

            return matches;
        }

        private static bool Match(StreetName streetName, string search, SanitizerBase sanitizer, Func<string, string, bool> comparer)
        {
            var sanitized = sanitizer.Sanitize(streetName.NameDutch ?? string.Empty, search);
            var result = comparer(sanitized.StreetName, sanitized.Search);
            if (result)
            {
                return true;
            }

            sanitized = sanitizer.Sanitize(streetName.NameFrench ?? string.Empty, search);
            result = comparer(sanitized.StreetName, sanitized.Search);
            if (result)
            {
                return true;
            }

            sanitized= sanitizer.Sanitize(streetName.NameGerman ?? string.Empty, search);
            result = comparer(sanitized.StreetName, sanitized.Search);
            if (result)
            {
                return true;
            }

            sanitized = sanitizer.Sanitize(streetName.NameEnglish ?? string.Empty, search);
            result = comparer(sanitized.StreetName, sanitized.Search);
            if (result)
            {
                return true;
            }

            return false;
        }
    }
}
