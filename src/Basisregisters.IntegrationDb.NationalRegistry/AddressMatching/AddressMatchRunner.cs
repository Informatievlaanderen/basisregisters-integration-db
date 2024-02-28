namespace Basisregisters.IntegrationDb.NationalRegistry.AddressMatching
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Model;
    using Repositories;

    public class AddressMatchRunner
    {
        private readonly AddressRepository _repo;

        public AddressMatchRunner(string connectionString)
        {
            _repo = new AddressRepository(connectionString);
        }

        public (IEnumerable<FlatFileRecordWithAddress> Matched, IEnumerable<FlatFileRecordWithStreetNames> Unmatched) Match(
            List<FlatFileRecordWithStreetNames> flatFileRecords)
        {
            var matched = flatFileRecords.ToDictionary(
                x => x,
                _ => new List<Address>());

            var groupedRecords = flatFileRecords
                .GroupBy(x => new { x.Record.NisCode, x.Record.PostalCode, x.Record.StreetName, x.Record.HouseNumber })
                .ToList();

            var allAddresses = _repo.GetAll()
                .GroupBy(x => x.StreetNamePersistentLocalId)
                .ToDictionary(
                    x => x.Key,
                    x => x.ToList());

            Parallel.ForEach(
                groupedRecords,
                new ParallelOptions { MaxDegreeOfParallelism = 50 },
                records =>
                {
                    var streetNamePersistentLocalIds = records.First().StreetNames.Select(x => x.StreetNamePersistentLocalId).ToList();

                    var addresses = new List<Address>();
                    foreach (var streetNamePersistentLocalId in streetNamePersistentLocalIds)
                    {
                        var pattern = $"^{records.Key.HouseNumber.TrimStart('0')}[^0-9]*$";
                        var houseNumberRegex = new Regex(pattern, RegexOptions.IgnoreCase);

                        if (allAddresses.TryGetValue(streetNamePersistentLocalId, out var streetNameAddresses))
                        {
                            addresses.AddRange(streetNameAddresses
                                .Where(x =>
                                    x.PostalCode == records.Key.PostalCode
                                    && houseNumberRegex.IsMatch(x.HouseNumber)));
                        }
                    }

                    // var addresses = _repo.GetAddresses(
                    //         records.First().StreetNames.Select(x => x.StreetNamePersistentLocalId).ToList(),
                    //         records.Key.PostalCode,
                    //         records.Key.HouseNumber)
                    //     .ToList();

                    foreach (var record in records)
                    {
                        var nationalRegistryAddress = new NationalRegistryAddress(record.Record);

                        var matches = addresses
                            .Where(x => nationalRegistryAddress.HouseNumberBoxNumbers is not null &&
                                        nationalRegistryAddress.HouseNumberBoxNumbers
                                            .GetValues()
                                            .Any(y =>
                                                string.Equals(y.HouseNumber, x.HouseNumber, StringComparison.InvariantCultureIgnoreCase)
                                                && string.Equals(y.BoxNumber, x.BoxNumber?.TrimStart('0'), StringComparison.InvariantCultureIgnoreCase)))
                            .ToList();

                        if (matches.Any())
                        {
                            foreach (var match in matches)
                            {
                                matched[record].Add(match);
                            }
                        }
                    }
                });

            try
            {
                var recordsWithMultipleAddresses = matched
                    .Where(x => x.Value.Count > 1)
                    .ToList();

                File.AppendAllLines(
                    @"C:\DV\inwonersaantallen\recordsWithMultipleAddresses.csv",
                    recordsWithMultipleAddresses.Select(x =>
                        $"{x.Key.Record.ToSafeString()};{x.Value.Select(y => y.AddressPersistentLocalId.ToString()).Aggregate((i, j) => $"{i};{j}")}"));

                // var addressesWithMultipleRecords = matched
                //     .SelectMany(x => x.Value)
                //     .GroupBy(x => x)
                //     .Where(x => x.Count() > 1)
                //     .ToList();

                var reversedDict = new Dictionary<Address, List<FlatFileRecordWithStreetNames>>();

                foreach (var kvp in matched)
                {
                    foreach (var value in kvp.Value)
                    {
                        if (!reversedDict.ContainsKey(value))
                        {
                            reversedDict[value] = new List<FlatFileRecordWithStreetNames>();
                        }
                        reversedDict[value].Add(kvp.Key);
                    }
                }

                var recordsForAddresses = reversedDict
                    .Where(x => x.Value.Count > 1)
                    .ToDictionary(x => x.Key, x => x.Value);

                // var recordsForAddresses = addressesWithMultipleRecords
                //     .Select(group => group.Key)
                //     .ToDictionary(
                //         address => address,
                //         address => matched
                //             .Where(x => x.Value.Contains(address))
                //             .Select(x => x.Key)
                //             .ToList());

                File.AppendAllLines(
                    @"C:\DV\inwonersaantallen\addressesWithMultipleRecords.csv",
                    recordsForAddresses.Select(x =>
                        $"{x.Key.AddressPersistentLocalId};{x.Value.Select(y => y.Record.ToSafeString()).Aggregate((i, j) => $"{i};{j}")}"));
            }
            catch (Exception e)
            { }

            var unmatched = matched
                .Where(x => !x.Value.Any())
                .Select(x => x.Key)
                .ToList();

            var flatFileRecordWithAddress = matched
                .Where(x => x.Value.Any())
                .Select(x => new FlatFileRecordWithAddress
                {
                    Address = x.Value.First(),
                    FlatFileRecordWithStreetNames = x.Key
                })
                .ToList();

            return (flatFileRecordWithAddress, unmatched);
        }
    }

    public class FlatFileRecordWithAddress
    {
        public FlatFileRecordWithStreetNames FlatFileRecordWithStreetNames { get; set; }
        public Address Address { get; set; }
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
