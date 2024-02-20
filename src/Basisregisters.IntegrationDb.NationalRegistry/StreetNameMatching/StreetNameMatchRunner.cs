namespace Basisregisters.IntegrationDb.NationalRegistry.StreetNameMatching
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
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
                    var match = matcher.MatchStreetName(nisCode, streetName);
                    if (match.Any())
                    {
                        matched[nisCode].Add(streetName);
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

            File.WriteAllLines(@"C:\DV\inwonersaantallen\unmatched.csv",
                unmatched
                    .SelectMany(x => x.Value.Select(y => new { NisCode = x.Key, StreetName = y }))
                    .Select(x => $"{x.NisCode};{x.StreetName}")
                    .Distinct());
        }
    }
}
