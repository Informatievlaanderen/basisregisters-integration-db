namespace Basisregisters.IntegrationDb.NationalRegistry.Model
{
    using Repositories;

    public class AddressWithRegisteredCount
    {
        public AddressWithRegisteredCount(
            FlatFileRecord? flatFileRecord,
            Address address,
            StreetName streetName,
            string houseNumberBoxNumberTypes,
            int? registeredCount)
        {
            FlatFileRecord = flatFileRecord;
            Address = address;
            StreetName = streetName;
            HouseNumberBoxNumberTypes = houseNumberBoxNumberTypes;
            RegisteredCount = registeredCount;
        }
        public FlatFileRecord? FlatFileRecord { get; set; }
        public Address Address { get; set; }
        public StreetName StreetName { get; set; }
        public string HouseNumberBoxNumberTypes { get; set; }

        public int? RegisteredCount { get; set; }
    }
}
