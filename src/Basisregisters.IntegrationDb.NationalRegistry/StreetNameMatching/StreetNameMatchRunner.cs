namespace Basisregisters.IntegrationDb.NationalRegistry.StreetNameMatching
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Repositories;

    public class StreetNameMatchRunner
    {
        private readonly StreetNameRepository _repo;

        public StreetNameMatchRunner(string connectionString)
        {
            _repo = new StreetNameRepository(connectionString);
        }

        public void Match(List<NisCodeStreetNameRecord> streetNameRecords)
        {
            var streetNamesByNisCode = streetNameRecords
                .GroupBy(x => x.NisCode)
                .ToDictionary(
                    x => x.Key,
                    x => x.Select(y => y.StreetName).ToList());

            var matched = new Dictionary<string, List<StreetNameMatches>>();
            var unmatched = new Dictionary<string, List<string>>();

            foreach (var (nisCode, streetNames) in streetNamesByNisCode)
            {
                var dbStreetNames = _repo.GetStreetNamesByNisCode(nisCode);

                var matcher = new StreetNameMatcher(dbStreetNames);
                matched.Add(nisCode, new List<StreetNameMatches>());
                unmatched.Add(nisCode, new List<string>());
                foreach (var streetName in streetNames)
                {
                    var match = matcher.MatchStreetName(nisCode, streetName).ToList();
                    if (match.Any())
                    {
                        matched[nisCode].Add(new StreetNameMatches(nisCode, streetName, match));
                    }
                    else
                    {
                        if (streetName == "INSCHRIJVING OP VERKLARING"
                            || streetName.StartsWith("KB ")
                            || streetName.StartsWith("NONRESIDENT")
                            || streetName.StartsWith("INSCRIPTION SUR DECLARATION")
                            || streetName.StartsWith("NIET-INWONER"))
                        {
                            continue;
                        }
                        unmatched[nisCode].Add(streetName);
                    }
                }
            }

            // var matchesWithDifferentNaming = new List<StreetNameMatches>();
            var result = new List<(string NisCode, StreetName StreetName, List<string> Searches)>();
            foreach (var nisCode in matched)
            {
                // Probleem 1: heeft een zoekresultaat meerdere matches met andere schrijfwijze
                // foreach (var match in nisCode.Value.Where(x => x.HasDifferentNaming))
                // {
                //     matchesWithDifferentNaming.Add(match);
                // }

                // Probleem 2: is een database straatnaam gematched met meerdere zoek straatnamen waarbij het niet gaat over een homoniemtoevoeging     .ToList();
                foreach (var streetName in nisCode.Value.SelectMany(x => x.Matched).Distinct())
                {
                    var occurances = nisCode.Value
                        .Where(x => x.Matched.Contains(streetName))
                        .ToList();

                    if (occurances.Count > 1)
                    {
                        result.Add((nisCode.Key, streetName, occurances.Select(x => x.Search).ToList()));
                    }
                }
            }

            File.WriteAllLines(@"C:\DV\inwonersaantallen\doubles.csv",
                result
                    .Select(x => $"{x.NisCode};{x.StreetName.NameDutch};{x.StreetName.HomonymAdditionDutch};{x.Searches.Aggregate((i,j) => $"{i},{j}")}")
                    .Distinct());

            // File.WriteAllLines(@"C:\DV\inwonersaantallen\unmatched.csv",
            //     unmatched
            //         .SelectMany(x => x.Value.Select(y => new { NisCode = x.Key, StreetName = y }))
            //         .Select(x => $"{x.NisCode};{x.StreetName}")
            //         .Distinct());
        }
    }

    public sealed record StreetNameMatches(
        string NisCode,
        string Search,
        List<StreetName> Matched)
    {
        public bool HasDifferentNaming => Matched
            .GroupBy(x => x.NameDutch)
            .Count() > 1;

        public bool HasHomonymAdditionFound => Matched
            .Any(x =>
                !string.IsNullOrWhiteSpace(x.HomonymAdditionDutch)
                || !string.IsNullOrWhiteSpace(x.HomonymAdditionFrench)
                || !string.IsNullOrWhiteSpace(x.HomonymAdditionGerman)
                || !string.IsNullOrWhiteSpace(x.HomonymAdditionEnglish));
    }
}
