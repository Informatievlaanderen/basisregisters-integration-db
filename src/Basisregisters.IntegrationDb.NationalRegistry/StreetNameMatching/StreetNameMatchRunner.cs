﻿namespace Basisregisters.IntegrationDb.NationalRegistry.StreetNameMatching
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Extensions;
    using Model;
    using Repositories;

    public class StreetNameMatchRunner
    {
        private readonly IStreetNameRepository _repo;

        public StreetNameMatchRunner(IStreetNameRepository streetNameRepository)
        {
            _repo = streetNameRepository;
        }

        public (IList<FlatFileRecordWithStreetNames> Matched, IList<FlatFileRecord> Unmatched) Match(IEnumerable<FlatFileRecord> flatFileRecords)
        {
            var streetNames = _repo.GetStreetNames();
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
                var dbStreetNames = streetNames
                    .Where(x => x.NisCode.Equals(nisCode, StringComparison.InvariantCultureIgnoreCase))
                    .ToList();

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

            return (matched.ToList(), unmatched.ToList());
        }
    }
}
