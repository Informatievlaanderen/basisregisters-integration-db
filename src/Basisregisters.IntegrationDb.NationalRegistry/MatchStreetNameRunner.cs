namespace Basisregisters.IntegrationDb.NationalRegistry
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using FlatFiles;
    using Matching;
    using Model;
    using Repositories;

    public class MatchStreetNameRunner
    {
        private readonly StreetNameRepository _repo;

        public MatchStreetNameRunner(string connectionString)
        {
            _repo = new StreetNameRepository(connectionString);
        }

        public void Match(string path)
        {
            var streetNamesByNisCode = ReadNisCodeStreetNameRecords(path)
                .GroupBy(x => x.NisCode)
                .ToDictionary(
                    x => x.Key,
                    x => x.Select(y => y.StreetName).ToList());

            var matched = new Dictionary<string, List<string>>();
            var unmatched = new Dictionary<string, List<string>>();

            foreach (var (nisCode, streetNames) in streetNamesByNisCode)
            {
                var dbStreetNames = _repo.GetStreetNamesByNisCode(nisCode);

                var matcher = new StreetNameMatcher(dbStreetNames);
                matched.Add(nisCode, new List<string>());
                unmatched.Add(nisCode, new List<string>());
                foreach (var streetName in streetNames)
                {
                    var match = matcher.MatchStreetName(streetName);
                    if (match.Any())
                    {
                        matched[nisCode].Add(streetName);
                    }
                    else
                    {
                        if (streetName == "INSCHRIJVING OP VERKLARING" || streetName.StartsWith("KB ")|| streetName.StartsWith("NONRESIDENT"))
                        {
                            continue;
                        }
                        unmatched[nisCode].Add(streetName);
                    }
                }
            }

            // File.WriteAllLines(@"C:\",
            //     matched
            //         .SelectMany(x => x.Value.Select(y => new { NisCode = x.Key, StreetName = y }))
            //         .Select(x => $"{x.NisCode};{x.StreetName}")
            //         .Distinct());
            File.WriteAllLines(@"C:",
                unmatched
                    .SelectMany(x => x.Value.Select(y => new { NisCode = x.Key, StreetName = y }))
                    .Select(x => $"{x.NisCode};{x.StreetName}")
                    .Distinct());
        }

        private static List<NisCodeStreetNameRecord> ReadNisCodeStreetNameRecords(string path)
        {
            using var reader = new StreamReader(path);
            var options = new DelimitedOptions()
            {
                IsFirstRecordSchema = false,
                Separator = ";"
            };
            var nisCodeStreetNameRecords = NisCodeStreetNameRecord.Mapper.Read(reader, options).ToList();
            return nisCodeStreetNameRecords;
        }
    }
}
