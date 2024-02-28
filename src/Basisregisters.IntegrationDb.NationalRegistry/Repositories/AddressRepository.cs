namespace Basisregisters.IntegrationDb.NationalRegistry.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Dapper;
    using Npgsql;

    public interface IAddressRepository
    {
        List<Address> GetAll();
        IEnumerable<Address> GetAddresses(List<int> streetNamePersistentLocalIds, string postalCode, string houseNumber);
    }

    public class AddressRepository : IAddressRepository
    {
        private readonly string _connectionString;

        public AddressRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Address> GetAll()
        {
            const string sql = @"select
            persistent_local_id as AddressPersistentLocalId
            , street_name_persistent_local_id as StreetNamePersistentLocalId
            , house_number as HouseNumber
            , box_number as BoxNumber
            , postal_code as PostalCode
            from integration_address.address_latest_items as a
            where a.removed = false and (oslo_status = 'InGebruik' or oslo_status = 'Voorgesteld')
            ";

            using var connection = new NpgsqlConnection(_connectionString);

            var addresses = connection.Query<Address>(sql);

            return addresses.ToList();
        }

        public IEnumerable<Address> GetAddresses(List<int> streetNamePersistentLocalIds, string postalCode, string houseNumber)
        {
            const string sql = @"select
            persistent_local_id as AddressPersistentLocalId
            , street_name_persistent_local_id as StreetNamePersistentLocalId
            , house_number as HouseNumber
            , box_number as BoxNumber
            , postal_code as PostalCode
            from integration_address.address_latest_items as a
            where
            a.street_name_persistent_local_id = ANY(@StreetNamePersistentLocalIds)
            and a.house_number ~ @HouseNumber
            and a.postal_code = @PostalCode
            and a.removed = false and (oslo_status = 'InGebruik' or oslo_status = 'Voorgesteld')";

            using var connection = new NpgsqlConnection(_connectionString);

            var addresses = connection.Query<Address>(sql, new
            {
                StreetNamePersistentLocalIds = streetNamePersistentLocalIds,
                PostalCode = postalCode,
                HouseNumber = $"^{houseNumber.TrimStart('0')}[[:alpha:]_]*"
            }, commandTimeout: 1000);

            return addresses;
        }
    }

    public class Address
    {
        public Address(
            int addressPersistentLocalId,
            int streetNamePersistentLocalId,
            string houseNumber,
            string? boxNumber,
            string postalCode)
        {
            AddressPersistentLocalId = addressPersistentLocalId;
            StreetNamePersistentLocalId = streetNamePersistentLocalId;
            HouseNumber = houseNumber;
            BoxNumber = boxNumber;
            PostalCode = postalCode;
        }

        public int AddressPersistentLocalId { get; set; }
        public int StreetNamePersistentLocalId { get; set; }
        public string PostalCode { get; set; }
        public string HouseNumber { get; set; }
        public string? BoxNumber { get; set; }
    }
}
