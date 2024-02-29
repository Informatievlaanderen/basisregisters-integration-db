namespace Basisregisters.IntegrationDb.NationalRegistry.AddressMatching
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Model;
    using Repositories;

    public class AddressMatchRunner
    {
        private readonly AddressRepository _repo;

        public AddressMatchRunner(
            string connectionString)
        {
            _repo = new AddressRepository(connectionString);
        }

        public AddressMatchResult Match(IList<FlatFileRecordWithStreetNames> flatFileRecords)
        {
            var matchesPerRecord = flatFileRecords.ToDictionary(
                x => x,
                _ => new List<(Address Address, string HouseNumberBoxNumberType)>());

            var recordsPerHouseNumber = flatFileRecords
                .GroupBy(x => new { x.Record.NisCode, x.Record.PostalCode, x.Record.StreetName, x.Record.HouseNumber })
                .ToList();

            var addressesPerStreetName = _repo.GetAll()
                .GroupBy(x => x.StreetNamePersistentLocalId)
                .ToDictionary(
                    x => x.Key,
                    x => x.ToList());

            Parallel.ForEach(
                recordsPerHouseNumber,
                new ParallelOptions { MaxDegreeOfParallelism = 50 },
                records =>
                {
                    var streetNamePersistentLocalIds = records.First().StreetNames.Select(x => x.StreetNamePersistentLocalId).ToList();

                    var addresses = FilterAddressesByHouseNumber(
                        addressesPerStreetName,
                        streetNamePersistentLocalIds,
                        records.Key.PostalCode,
                        records.Key.HouseNumber);

                    foreach (var record in records)
                    {
                        var nationalRegistryAddress = new NationalRegistryAddress(record.Record);

                        var matches = addresses
                            .Where(x =>
                                nationalRegistryAddress.HouseNumberBoxNumbers is not null
                                && nationalRegistryAddress.HouseNumberBoxNumbers
                                    .GetValues()
                                    .Any(y =>
                                        string.Equals(y.HouseNumber, x.HouseNumber, StringComparison.InvariantCultureIgnoreCase)
                                        && string.Equals(y.BoxNumber, x.BoxNumber?.TrimStart('0'), StringComparison.InvariantCultureIgnoreCase)))
                            .ToList();

                        foreach (var matchedAddress in matches)
                        {
                            matchesPerRecord[record].Add((
                                matchedAddress,
                                nationalRegistryAddress.HouseNumberBoxNumbersType!.Name));
                        }
                    }
                });

            var matchedRecords = GetMatchedRecords(matchesPerRecord);
            var unmatchedRecords = GetUnmatchedRecords(matchesPerRecord);
            var recordsMatchedWithMultipleAddresses = GetRecordsMatchedWithMultipleAddresses(matchesPerRecord);
            var addressesMatchedWithMultipleRecords = GetAddressesMatchedWithMultipleRecords(matchesPerRecord);

            return new AddressMatchResult(
                matchedRecords,
                unmatchedRecords,
                recordsMatchedWithMultipleAddresses,
                addressesMatchedWithMultipleRecords);
        }

        private static List<Address> FilterAddressesByHouseNumber(
            IReadOnlyDictionary<int, List<Address>> addressesPerStreetName,
            IEnumerable<int> streetNamePersistentLocalIds,
            string postalCode,
            string houseNumber)
        {
            var addresses = new List<Address>();
            foreach (var streetNamePersistentLocalId in streetNamePersistentLocalIds)
            {
                var pattern = $"^{houseNumber.TrimStart('0')}[^0-9]*$";
                var houseNumberRegex = new Regex(pattern, RegexOptions.IgnoreCase);

                if (addressesPerStreetName.TryGetValue(streetNamePersistentLocalId, out var streetNameAddresses))
                {
                    addresses.AddRange(streetNameAddresses
                        .Where(x =>
                            x.PostalCode == postalCode
                            && houseNumberRegex.IsMatch(x.HouseNumber)));
                }
            }

            return addresses;
        }

        private static IEnumerable<FlatFileRecordWithAddress> GetMatchedRecords(
            Dictionary<FlatFileRecordWithStreetNames, List<(Address Address, string HouseNumberBoxNumberType)>> matchesPerRecord)
        {
            return matchesPerRecord
                .Where(x => x.Value.Any())
                .Select(x => new FlatFileRecordWithAddress
                {
                    Address = x.Value.First().Address,
                    FlatFileRecordWithStreetNames = x.Key,
                    HouseNumberBoxNumberType = x.Value.First().HouseNumberBoxNumberType
                })
                .ToList();
        }

        private static IEnumerable<FlatFileRecordWithStreetNames> GetUnmatchedRecords(
            Dictionary<FlatFileRecordWithStreetNames, List<(Address Address, string HouseNumberBoxNumberType)>> matchesPerRecord)
        {
            return matchesPerRecord
                .Where(x => !x.Value.Any())
                .Select(x => x.Key)
                .ToList();
        }

        private static IDictionary<FlatFileRecordWithStreetNames, List<Address>> GetRecordsMatchedWithMultipleAddresses(
            IDictionary<FlatFileRecordWithStreetNames, List<(Address Address, string HouseNumberBoxNumberType)>> matchesPerRecord)
        {
            return matchesPerRecord
                .Where(x => x.Value.Count > 1)
                .ToDictionary(
                    x => x.Key,
                    x => x.Value.Select(y => y.Address).ToList());
        }

        private static IDictionary<Address, List<FlatFileRecordWithStreetNames>> GetAddressesMatchedWithMultipleRecords(
            IDictionary<FlatFileRecordWithStreetNames, List<(Address Address, string HouseNumberBoxNumberType)>> matchesPerRecord)
        {
            var matchesPerRecordReversed = new Dictionary<Address, List<FlatFileRecordWithStreetNames>>();
            foreach (var kvp in matchesPerRecord)
            {
                foreach (var value in kvp.Value)
                {
                    if (!matchesPerRecordReversed.ContainsKey(value.Address))
                    {
                        matchesPerRecordReversed[value.Address] = new List<FlatFileRecordWithStreetNames>();
                    }

                    matchesPerRecordReversed[value.Address].Add(kvp.Key);
                }
            }

            return matchesPerRecordReversed
                .Where(x => x.Value.Count > 1)
                .ToDictionary(x => x.Key, x => x.Value);
        }
    }
}
