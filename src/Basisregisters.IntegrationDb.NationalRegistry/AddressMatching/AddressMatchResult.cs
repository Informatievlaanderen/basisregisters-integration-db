namespace Basisregisters.IntegrationDb.NationalRegistry.AddressMatching
{
    using System.Collections.Generic;
    using Model;
    using Repositories;

    public class AddressMatchResult
    {
        public IEnumerable<FlatFileRecordWithAddress> MatchedRecords { get; }
        public IEnumerable<FlatFileRecordWithStreetNames> UnmatchedRecords { get; }

        public IDictionary<FlatFileRecordWithStreetNames, List<Address>> RecordsMatchedWithMultipleAddresses { get; }
        public IDictionary<Address, List<FlatFileRecordWithStreetNames>> AddressesMatchedWithMultipleRecords { get; }

        public AddressMatchResult(
            IEnumerable<FlatFileRecordWithAddress> matchedRecords,
            IEnumerable<FlatFileRecordWithStreetNames> unmatchedRecords,
            IDictionary<FlatFileRecordWithStreetNames, List<Address>> recordsMatchedWithMultipleAddresses,
            IDictionary<Address, List<FlatFileRecordWithStreetNames>> addressesMatchedWithMultipleRecords)
        {
            MatchedRecords = matchedRecords;
            UnmatchedRecords = unmatchedRecords;
            RecordsMatchedWithMultipleAddresses = recordsMatchedWithMultipleAddresses;
            AddressesMatchedWithMultipleRecords = addressesMatchedWithMultipleRecords;
        }
    }
}
