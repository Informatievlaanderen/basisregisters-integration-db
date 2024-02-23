namespace Basisregisters.IntegrationDb.NationalRegistry.StreetNameMatching
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Extensions;
    using Matchers;
    using Repositories;
    using Sanitizers;

    public class StreetNameMatcher
    {
        private readonly IEnumerable<StreetName> _streetNames;

        private readonly IList<IMatcher> _matchers;
        private readonly IList<SanitizerBase> _sanitizers;

        public StreetNameMatcher(
            IEnumerable<StreetName> streetNames,
            int maxLevenshteinDistanceInPercentage = 10)
        {
            _streetNames = streetNames.ToList();

            foreach (var streetName in _streetNames)
            {
                streetName.NameDutch = streetName.NameDutch?.RemoveDiacritics().RemoveQuotations().RemoveSpecialCharacters() ?? string.Empty;
                streetName.NameFrench = streetName.NameFrench?.RemoveDiacritics().RemoveQuotations().RemoveSpecialCharacters() ?? string.Empty;
                streetName.NameGerman = streetName.NameGerman?.RemoveDiacritics().RemoveQuotations().RemoveSpecialCharacters() ?? string.Empty;
                streetName.NameEnglish = streetName.NameEnglish?.RemoveDiacritics().RemoveQuotations().RemoveSpecialCharacters() ?? string.Empty;
            }

            _matchers = new List<IMatcher>
            {
                new ExactMatcher(),
                new LevenshteinMatcher(maxLevenshteinDistanceInPercentage),
                new RegexMatcher()
            };

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
                foreach (var matcher in _matchers)
                {
                    var matches = Match(searchValue, sanitizer, matcher.Match);
                    if (matches.Any())
                    {
                        return matches;
                    }
                }
            }

            return new List<StreetName>();
        }

        private List<StreetName> Match(string search, SanitizerBase sanitizer, Func<string, string, bool> comparer)
        {
            var matches = _streetNames
                .Where(x => Match(x, search, sanitizer, comparer))
                .ToList();

            if (matches.Any())
            {
                return matches;
            }

            var sanitizedSearch = search
                .RemoveDiacritics()
                .RemoveQuotations()
                .RemoveSpecialCharacters();

            matches = _streetNames
                .Where(x => Match(x, sanitizedSearch, sanitizer, comparer))
                .ToList();

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
