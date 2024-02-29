namespace Basisregisters.IntegrationDb.NationalRegistry.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Dapper;
    using NetTopologySuite.Geometries;
    using Npgsql;

    public interface IAddressRepository
    {
        List<Address> GetAll();
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
            a.persistent_local_id as AddressPersistentLocalId
            , a.street_name_persistent_local_id as StreetNamePersistentLocalId
            , a.house_number as HouseNumber
            , a.box_number as BoxNumber
            , a.postal_code as PostalCode
            , a.oslo_status as Status
            , a.oslo_position_method as Method
            , a.oslo_position_specification as Specification
            , a.geometry as Position
            from integration_address.address_latest_items as a
            JOIN integration_streetname.streetname_latest_items as s on a.street_name_persistent_local_id = s.persistent_local_id
            where s.nis_code = '23027'
            and a.removed = false and (a.oslo_status = 'InGebruik' or a.oslo_status = 'Voorgesteld')
            ;";

            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            connection.TypeMapper.UseNetTopologySuite();

            var addresses = connection.Query<Address>(sql);

            return addresses.ToList();
        }
    }

    public class Address
    {
        public Address(
            int addressPersistentLocalId,
            int streetNamePersistentLocalId,
            string houseNumber,
            string? boxNumber,
            string postalCode,
            string status,
            string method,
            string specification,
            Point position)
        {
            AddressPersistentLocalId = addressPersistentLocalId;
            StreetNamePersistentLocalId = streetNamePersistentLocalId;
            HouseNumber = houseNumber;
            BoxNumber = boxNumber;
            PostalCode = postalCode;
            Status = status;
            Method = method;
            Specification = specification;
            Position = position;
        }

        public Address()
        { }

        public int AddressPersistentLocalId { get; set; }
        public int StreetNamePersistentLocalId { get; set; }
        public string PostalCode { get; set; }
        public string HouseNumber { get; set; }
        public string? BoxNumber { get; set; }
        public string Status { get; set; }
        public string Method { get; set; }
        public string Specification { get; set; }
        public Point Position { get; set; }
    }
}
