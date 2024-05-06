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
        private readonly IEnumerable<MatchingStreetName> _streetNames;

        private readonly IList<IMatcher> _matchers;
        private readonly IList<SanitizerBase> _sanitizers;

        public StreetNameMatcher(
            IEnumerable<StreetName> streetNames,
            int maxLevenshteinDistanceInPercentage = 10)
        {
            _streetNames = streetNames
                .Select(streetName => new MatchingStreetName(streetName)
                {
                    SanitizedNameDutch = streetName.NameDutch?.RemoveDiacritics().RemoveQuotations().RemoveSpecialCharacters() ?? string.Empty,
                    SanitizedNameFrench = streetName.NameFrench?.RemoveDiacritics().RemoveQuotations().RemoveSpecialCharacters() ?? string.Empty,
                    SanitizedNameGerman = streetName.NameGerman?.RemoveDiacritics().RemoveQuotations().RemoveSpecialCharacters() ?? string.Empty,
                    SanitizedNameEnglish = streetName.NameEnglish?.RemoveDiacritics().RemoveQuotations().RemoveSpecialCharacters() ?? string.Empty
                }).ToList();
            
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
                new TrimEndSanitizer(new SanitizerBase[] { new AbbreviationSanitizer() }),
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
                .Select(x => x.StreetName)
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
                .Select(x => x.StreetName)
                .ToList();

            return matches;
        }

        private static bool Match(MatchingStreetName streetName, string search, SanitizerBase sanitizer, Func<string, string, bool> comparer)
        {
            var sanitized = sanitizer.Sanitize(streetName.SanitizedNameDutch, search);
            var result = comparer(sanitized.StreetName, sanitized.Search);
            if (result)
            {
                return true;
            }

            sanitized = sanitizer.Sanitize(streetName.SanitizedNameFrench, search);
            result = comparer(sanitized.StreetName, sanitized.Search);
            if (result)
            {
                return true;
            }

            sanitized= sanitizer.Sanitize(streetName.SanitizedNameGerman, search);
            result = comparer(sanitized.StreetName, sanitized.Search);
            if (result)
            {
                return true;
            }

            sanitized = sanitizer.Sanitize(streetName.SanitizedNameEnglish, search);
            result = comparer(sanitized.StreetName, sanitized.Search);
            if (result)
            {
                return true;
            }

            return false;
        }

        private sealed record MatchingStreetName(StreetName StreetName)
        {
            public string SanitizedNameDutch { get; init; }
            public string SanitizedNameFrench { get; init; }
            public string SanitizedNameGerman { get; init; }
            public string SanitizedNameEnglish { get; init; }
        }
    }
}
