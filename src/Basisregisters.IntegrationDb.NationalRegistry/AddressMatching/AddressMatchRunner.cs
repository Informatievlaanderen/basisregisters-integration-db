namespace Basisregisters.IntegrationDb.NationalRegistry.AddressMatching
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Basisregisters.IntegrationDb.NationalRegistry.Extensions;
    using Basisregisters.IntegrationDb.NationalRegistry.Model;
    using Basisregisters.IntegrationDb.NationalRegistry.Repositories;
    using StreetNameMatching;

    public class AddressMatchRunner
    {
        private readonly AddressRepository _repo;

        public AddressMatchRunner(string connectionString)
        {
            _repo = new AddressRepository(connectionString);
        }

        public (IEnumerable<FlatFileRecordWithAddress> Matched, IEnumerable<FlatFileRecordWithStreetNames> Unmatched) Match(List<FlatFileRecordWithStreetNames> flatFileRecords)
        {
            var matched = flatFileRecords.ToDictionary(
                x => x,
                _ => new List<Address>());

            var groupedRecords = flatFileRecords
                .GroupBy(x => new { x.Record.NisCode, x.Record.PostalCode, x.Record.StreetName, x.Record.HouseNumber });

            Parallel.ForEach(groupedRecords, records =>
            {
                var addresses = _repo.GetAddresses(
                        records.First().StreetNames.Select(x => x.StreetNamePersistentLocalId).ToList(),
                        records.Key.PostalCode,
                        records.Key.HouseNumber)
                    .ToList();

                foreach (var record in records)
                {
                    var nationalRegistryAddress = new NationalRegistryAddress(record.Record);

                    var matches = addresses
                        .Where(x => nationalRegistryAddress.HouseNumberBoxNumbers is not null &&
                                    nationalRegistryAddress.HouseNumberBoxNumbers
                            .GetValues()
                            .Any(y =>
                                string.Equals(y.HouseNumber, x.HouseNumber, StringComparison.InvariantCultureIgnoreCase)
                                && string.Equals(y.BoxNumber, x.BoxNumber, StringComparison.InvariantCultureIgnoreCase)))
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

            var unmatched = matched
                .Where(x => !x.Value.Any())
                .Select(x => x.Key)
                .ToList();

            var flatFileRecordWithAddress = matched
                .Where(x => x.Value.Any())
                .Select(x => new FlatFileRecordWithAddress()
                {
                    Address = x.Value.First(),
                    FlatFileRecordWithStreetNames = x.Key
                })
                .ToList();

            return (flatFileRecordWithAddress, unmatched);

            // var matched = flatFileRecords.ToDictionary(
            //     x => x,
            //     _ => new List<Address>());
            //
            // var streetNames = flatFileRecords
            //     .SelectMany(x => x.StreetNames)
            //     .Distinct()
            //     .ToList();

            // Parallel.ForEach(streetNames, streetName =>
            // {
            //     var addresses = _repo.GetAddresses(streetName.StreetNamePersistentLocalId, );
            //
            //     var nationalRegistryAddresses = flatFileRecords
            //         .Where(x => x.StreetNames.Contains(streetName))
            //         .Select(x => new
            //         {
            //             Record = x,
            //             Address = new NationalRegistryAddress(x.Record)
            //         })
            //         .ToList();
            //
            //     foreach (var address in addresses)
            //     {
            //         var matches = nationalRegistryAddresses
            //             .Where(x => x.Address.HouseNumberBoxNumbers
            //                 .GetValues()
            //                 .Any(y =>
            //                     string.Equals(y.HouseNumber, address.HouseNumber, StringComparison.InvariantCultureIgnoreCase)
            //                     && string.Equals(y.BoxNumber, address.BoxNumber, StringComparison.InvariantCultureIgnoreCase)))
            //             .ToList();
            //
            //         if (matches.Any())
            //         {
            //             foreach (var match in matches)
            //             {
            //                 matched[match.Record].Add(address);
            //             }
            //         }
            //     }
            // });
            //
            // var unmatched = matched
            //     .Where(x => !x.Value.Any())
            //     .Select(x => x.Key)
            //     .ToList();
            //
            // var flatFileRecordWithAddress = matched
            //     .Where(x => x.Value.Any())
            //     .Select(x => new FlatFileRecordWithAddress()
            //     {
            //         Address = x.Value.First(),
            //         FlatFileRecordWithStreetNames = x.Key
            //     })
            //     .ToList();
            //
            // return (flatFileRecordWithAddress, unmatched);

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
