namespace Basisregisters.IntegrationDb.NationalRegistry.StreetNameMatching
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Extensions;
    using Model;
    using Repositories;

    public class StreetNameMatchRunner
    {
        private readonly StreetNameRepository _repo;

        public StreetNameMatchRunner(string connectionString)
        {
            _repo = new StreetNameRepository(connectionString);
        }

        public (IEnumerable<FlatFileRecordWithStreetNames> Matched, IEnumerable<FlatFileRecord> Unmatched) Match(IEnumerable<FlatFileRecord> flatFileRecords)
        {
            var streetNamesByNisCode = flatFileRecords
                .GroupBy(x => x.NisCode)
                .ToDictionary(
                    x => x.Key,
                    x => x.Select(y => y).ToList());

            var matched = new ConcurrentBag<FlatFileRecordWithStreetNames>();
            var unmatched = new ConcurrentBag<FlatFileRecord>();

            Parallel.ForEach(streetNamesByNisCode, kvp =>
            {
                var nisCode = kvp.Key;
                var records = kvp.Value;

                var recordsByStreetName = records.GroupBy(x => x.StreetName);
                var dbStreetNames = _repo.GetStreetNamesByNisCode(nisCode);

                var matcher = new StreetNameMatcher(dbStreetNames);
                foreach (var streetNameRecord in recordsByStreetName)
                {
                    var dbMatches = matcher.MatchStreetName(nisCode, streetNameRecord.Key).ToList();
                    if (dbMatches.Any())
                    {
                        matched.AddRange(streetNameRecord.Select(flatFileRecord => new FlatFileRecordWithStreetNames(flatFileRecord, dbMatches)));
                    }
                    else
                    {
                        if (streetNameRecord.Key == "INSCHRIJVING OP VERKLARING"
                            || streetNameRecord.Key.StartsWith("KB ")
                            || streetNameRecord.Key.StartsWith("NONRESIDENT")
                            || streetNameRecord.Key.StartsWith("INSCRIPTION SUR DECLARATION")
                            || streetNameRecord.Key.StartsWith("NIET-INWONER"))
                        {
                            continue;
                        }
                        unmatched.AddRange(streetNameRecord);
                    }
                }
            });

            return (matched, unmatched);

            // // var matchesWithDifferentNaming = new List<StreetNameMatches>();
            // var result = new List<(string NisCode, StreetName StreetName, List<string> Searches)>();
            // foreach (var nisCode in matched)
            // {
            //     // Probleem 1: heeft een zoekresultaat meerdere matches met andere schrijfwijze
            //     // foreach (var match in nisCode.Value.Where(x => x.HasDifferentNaming))
            //     // {
            //     //     matchesWithDifferentNaming.Add(match);
            //     // }
            //
            //     // Probleem 2: is een database straatnaam gematched met meerdere zoek straatnamen waarbij het niet gaat over een homoniemtoevoeging     .ToList();
            //     foreach (var streetName in nisCode.Value.SelectMany(x => x.Matched).Distinct())
            //     {
            //         var occurances = nisCode.Value
            //             .Where(x => x.Matched.Contains(streetName))
            //             .ToList();
            //
            //         if (occurances.Count > 1)
            //         {
            //             result.Add((nisCode.Key, streetName, occurances.Select(x => x.Search).ToList()));
            //         }
            //     }
            // }
            //
            // File.WriteAllLines(@"C:\Users\egonm\Documents\Digitaal Vlaanderen\inwonersaantallen\doubles2.csv",
            //     result
            //         .Select(x => $"{x.NisCode};{x.StreetName.NameDutch};{x.StreetName.HomonymAdditionDutch};{x.Searches.Aggregate((i,j) => $"{i},{j}")}")
            //         .Distinct());
            //
            // // File.WriteAllLines(@"C:\DV\inwonersaantallen\unmatched.csv",
            // //     unmatched
            // //         .SelectMany(x => x.Value.Select(y => new { NisCode = x.Key, StreetName = y }))
            // //         .Select(x => $"{x.NisCode};{x.StreetName}")
            // //         .Distinct());
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
