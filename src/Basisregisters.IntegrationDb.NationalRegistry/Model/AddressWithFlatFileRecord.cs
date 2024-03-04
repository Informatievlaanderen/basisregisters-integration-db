namespace Basisregisters.IntegrationDb.NationalRegistry.Model
{
    using Repositories;

    public class AddressWithFlatFileRecord
    {
        public FlatFileRecord? FlatFileRecord { get; set; }
        public Address Address { get; set; }
        public string HouseNumberBoxNumberType { get; set; }
    }
}
