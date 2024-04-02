namespace Basisregisters.IntegrationDb.NationalRegistry.AddressMatching
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Model;
    using Repositories;

    public class AddressMatchRunner
    {
        private readonly IAddressRepository _addressRepository;
        private readonly IList<StreetName> _streetNames;

        public AddressMatchRunner(
            IAddressRepository addressRepository,
            IStreetNameRepository streetNameRepository)
        {
            _addressRepository = addressRepository;
            _streetNames = streetNameRepository.GetStreetNames();
        }

        public AddressMatchResult Match(IList<FlatFileRecordWithStreetNames> flatFileRecords)
        {
            Dictionary<FlatFileRecordWithStreetNames, ConcurrentBag<(Address Address, string HouseNumberBoxNumberTypes)>> matchesPerRecord = flatFileRecords.ToDictionary(
                x => x,
                _ => new ConcurrentBag<(Address Address, string HouseNumberBoxNumberType)>());

            var recordsPerHouseNumber = flatFileRecords
                .GroupBy(x => new { x.Record.NisCode, x.Record.PostalCode, x.Record.StreetName, x.Record.HouseNumber })
                .ToList();

            var allAddresses = _addressRepository.GetAll();
            var addressesPerStreetName = allAddresses
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
                                nationalRegistryAddress.HouseNumberBoxNumbers.SelectMany(y => y.GetValues())
                                    .Any(z =>
                                        string.Equals(z.HouseNumber, x.HouseNumber, StringComparison.InvariantCultureIgnoreCase)
                                        && string.Equals(z.BoxNumber?.TrimStart('0'), x.BoxNumber?.TrimStart('0'), StringComparison.InvariantCultureIgnoreCase)))
                            .ToList();

                        foreach (var matchedAddress in matches)
                        {
                            matchesPerRecord[record].Add((
                                matchedAddress,
                                string.Join(',', nationalRegistryAddress.HouseNumberBoxNumbers.Select(x => x.GetType().Name))));
                        }
                    }
                });

            var addressesMatchedWithMultipleRecords = GetAddressesMatchedWithMultipleRecords(matchesPerRecord);
            var recordsMatchedWithMultipleAddresses = GetRecordsMatchedWithMultipleAddresses(matchesPerRecord);

            var matchedRecords = GetMatchedRecords(matchesPerRecord, addressesMatchedWithMultipleRecords, allAddresses);
            var unmatchedRecords = GetUnmatchedRecords(matchesPerRecord);

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

        private IList<AddressWithRegisteredCount> GetMatchedRecords(
            Dictionary<FlatFileRecordWithStreetNames, ConcurrentBag<(Address Address, string HouseNumberBoxNumberTypes)>> matchesPerRecord,
            IDictionary<Address, List<FlatFileRecordWithStreetNames>> addressesMatchedWithMultipleRecords,
            IEnumerable<Address> allAddresses)
        {
            var singleMatches = matchesPerRecord
                .Where(x =>
                    x.Value.Count == 1
                    && !addressesMatchedWithMultipleRecords.ContainsKey(x.Value.Single().Address));

            var reversed = new Dictionary<int, (
                FlatFileRecord Record,
                Address Address,
                string HouseNumberBoxNumberType)>();

            var boxNumberMatchesByHouseNumber = new Dictionary<int, List<(
                FlatFileRecord Record,
                Address Address,
                string HouseNumberBoxNumberType)>>();

            foreach (var (record, match) in singleMatches)
            {
                var address = match.Single();

                reversed.Add(
                    address.Address.AddressPersistentLocalId,
                    (record.Record, address.Address, HouseNumberBoxNumberType: address.HouseNumberBoxNumberTypes));

                if (address.Address.IsBoxNumber)
                {
                    if (boxNumberMatchesByHouseNumber.ContainsKey(address.Address.ParentPersistentLocalId!.Value))
                    {
                        boxNumberMatchesByHouseNumber[address.Address.ParentPersistentLocalId.Value].Add(
                            (record.Record, address.Address, HouseNumberBoxNumberType: address.HouseNumberBoxNumberTypes));
                    }
                    else
                    {
                        boxNumberMatchesByHouseNumber.Add(
                            address.Address.ParentPersistentLocalId.Value,
                            new List<(FlatFileRecord Record, Address Address, string HouseNumberBoxNumberType)>
                            {
                                (record.Record, address.Address, HouseNumberBoxNumberType: address.HouseNumberBoxNumberTypes)
                            }
                        );
                    }
                }
            }

            var results = new ConcurrentBag<AddressWithRegisteredCount>();

            var addressesPerStreetName = allAddresses
                .GroupBy(x => x.StreetNamePersistentLocalId)
                .ToList();

            Parallel.ForEach(
                addressesPerStreetName,
                new ParallelOptions { MaxDegreeOfParallelism = 50 },
                addresses =>
                {
                    foreach (var address in addresses)
                    {
                        var streetName = _streetNames.Single(x => x.StreetNamePersistentLocalId == address.StreetNamePersistentLocalId);

                        if (reversed.TryGetValue(address.AddressPersistentLocalId, out var match))
                        {
                            results.Add(new AddressWithRegisteredCount(
                                match.Record,
                                address,
                                streetName,
                                match.HouseNumberBoxNumberType,
                                match.Record.RegisteredCount));
                            continue;
                        }

                        if (address.IsHouseNumber
                            && boxNumberMatchesByHouseNumber.TryGetValue(address.AddressPersistentLocalId, out var boxNumbers)
                            && boxNumbers.Any(x => x.Address.ParentPersistentLocalId == address.AddressPersistentLocalId))
                        {
                            results.Add(new AddressWithRegisteredCount(
                                null,
                                address,
                                streetName,
                                string.Empty,
                                null));
                            continue;
                        }

                        if (address.IsBoxNumber
                            && reversed.ContainsKey(address.ParentPersistentLocalId!.Value))
                        {
                            results.Add(new AddressWithRegisteredCount(
                                null,
                                address,
                                streetName,
                                string.Empty,
                                null));
                            continue;
                        }

                        results.Add(new AddressWithRegisteredCount(
                            null,
                            address,
                            streetName,
                            string.Empty,
                            0));
                    }
                });

            return results.ToList();
        }

        private static IList<FlatFileRecordWithStreetNames> GetUnmatchedRecords(
            Dictionary<FlatFileRecordWithStreetNames, ConcurrentBag<(Address Address, string HouseNumberBoxNumberTypes)>> matchesPerRecord)
        {
            return matchesPerRecord
                .Where(x => !x.Value.Any())
                .Select(x => x.Key)
                .ToList();
        }

        private static IDictionary<FlatFileRecordWithStreetNames, List<Address>> GetRecordsMatchedWithMultipleAddresses(
            IDictionary<FlatFileRecordWithStreetNames, ConcurrentBag<(Address Address, string HouseNumberBoxNumberTypes)>> matchesPerRecord)
        {
            return matchesPerRecord
                .Where(x => x.Value.Count > 1)
                .ToDictionary(
                    x => x.Key,
                    x => x.Value.Select(y => y.Address).ToList());
        }

        private static IDictionary<Address, List<FlatFileRecordWithStreetNames>> GetAddressesMatchedWithMultipleRecords(
            IDictionary<FlatFileRecordWithStreetNames, ConcurrentBag<(Address Address, string HouseNumberBoxNumberTypes)>> matchesPerRecord)
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
