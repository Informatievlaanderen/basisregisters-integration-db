namespace Basisregisters.IntegrationDb.NationalRegistry.AddressMatching
{
    using System.Collections.Generic;
    using Model;
    using Repositories;

    public class AddressMatchResult
    {
        public IList<AddressWithRegisteredCount> AddressesWithRegisteredCount { get; }

        public IList<FlatFileRecordWithStreetNames> UnmatchedRecords { get; }
        public IDictionary<FlatFileRecordWithStreetNames, List<Address>> RecordsMatchedWithMultipleAddresses { get; }
        public IDictionary<Address, List<FlatFileRecordWithStreetNames>> AddressesMatchedWithMultipleRecords { get; }

        public AddressMatchResult(
            IList<AddressWithRegisteredCount> addressesWithRegisteredCount,
            IList<FlatFileRecordWithStreetNames> unmatchedRecords,
            IDictionary<FlatFileRecordWithStreetNames, List<Address>> recordsMatchedWithMultipleAddresses,
            IDictionary<Address, List<FlatFileRecordWithStreetNames>> addressesMatchedWithMultipleRecords)
        {
            AddressesWithRegisteredCount = addressesWithRegisteredCount;
            UnmatchedRecords = unmatchedRecords;
            RecordsMatchedWithMultipleAddresses = recordsMatchedWithMultipleAddresses;
            AddressesMatchedWithMultipleRecords = addressesMatchedWithMultipleRecords;
        }
    }
}
