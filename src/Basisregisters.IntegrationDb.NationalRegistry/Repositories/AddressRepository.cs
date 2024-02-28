namespace Basisregisters.IntegrationDb.NationalRegistry.Repositories
{
    using System.Collections.Generic;
    using Dapper;
    using Npgsql;

    public interface IAddressRepository
    {
        IEnumerable<Address> GetAddresses(List<int> streetNamePersistentLocalIds, string postalCode, string houseNumber);
    }

    public class AddressRepository : IAddressRepository
    {
        private readonly string _connectionString;

        public AddressRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IEnumerable<Address> GetAddresses(List<int> streetNamePersistentLocalIds, string postalCode, string houseNumber)
        {
            const string sql = @"select
            persistent_local_id as AddressPersistentLocalId
            , street_name_persistent_local_id as StreetNamePersistentLocalId
            , house_number as HouseNumber
            , box_number as BoxNumber
            from integration_address.address_latest_items as a
            where
            a.street_name_persistent_local_id = ANY(@StreetNamePersistentLocalIds)
            and a.house_number ~ @HouseNumber
            and a.postal_code = @PostalCode
            and a.removed = false and (oslo_status = 'InGebruik' or oslo_status = 'Voorgesteld')";

            using var connection = new NpgsqlConnection(_connectionString);

            var streetNames = connection.Query<Address>(sql, new
            {
                StreetNamePersistentLocalIds = streetNamePersistentLocalIds,
                PostalCode = postalCode,
                HouseNumber = $"^{houseNumber.TrimStart('0')}[[:alpha:]_]*"
            });

            return streetNames;
        }
    }

    public class Address
    {
        public Address(
            int addressPersistentLocalId,
            int streetNamePersistentLocalId,
            string houseNumber,
            string boxNumber)
        {
            AddressPersistentLocalId = addressPersistentLocalId;
            StreetNamePersistentLocalId = streetNamePersistentLocalId;
            HouseNumber = houseNumber;
            BoxNumber = boxNumber;
        }

        public int AddressPersistentLocalId { get; set; }
        public int StreetNamePersistentLocalId { get; set; }
        public string HouseNumber { get; set; }
        public string BoxNumber { get; set; }
    }
}
