namespace Basisregisters.IntegrationDb.NationalRegistry.Model
{
    using Repositories;

    public class FlatFileRecordWithAddress
    {
        public FlatFileRecordWithStreetNames FlatFileRecordWithStreetNames { get; set; }
        public Address Address { get; set; }
        public string HouseNumberBoxNumberType { get; set; }
    }
}
