namespace Basisregisters.IntegrationDb.NationalRegistry.Model
{
    using Repositories;

    public class AddressWithRegisteredCount
    {
        public AddressWithRegisteredCount(
            FlatFileRecord? flatFileRecord,
            Address address,
            StreetName streetName,
            string houseNumberBoxNumberType,
            int? registeredCount)
        {
            FlatFileRecord = flatFileRecord;
            Address = address;
            StreetName = streetName;
            HouseNumberBoxNumberType = houseNumberBoxNumberType;
            RegisteredCount = registeredCount;
        }
        public FlatFileRecord? FlatFileRecord { get; set; }
        public Address Address { get; set; }
        public StreetName StreetName { get; set; }
        public string HouseNumberBoxNumberType { get; set; }

        public int? RegisteredCount { get; set; }
    }
}
