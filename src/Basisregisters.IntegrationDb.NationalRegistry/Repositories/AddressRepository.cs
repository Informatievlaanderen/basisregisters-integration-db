namespace Basisregisters.IntegrationDb.NationalRegistry.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Dapper;
    using NetTopologySuite.Geometries;
    using Npgsql;

    public interface IAddressRepository
    {
        IList<Address> GetAll();
    }

    public class AddressRepository : IAddressRepository
    {
        private readonly string _connectionString;
        private IList<Address>? _addresses;

        public AddressRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IList<Address> GetAll()
        {
            if (_addresses is not null)
            {
                return _addresses;
            }

            const string sql = @"select
            a.persistent_local_id as AddressPersistentLocalId
            , a.street_name_persistent_local_id as StreetNamePersistentLocalId
            , a.parent_persistent_local_id as ParentPersistentLocalId
            , a.house_number as HouseNumber
            , a.box_number as BoxNumber
            , a.postal_code as PostalCode
            , a.oslo_status as Status
            , a.oslo_position_method as Method
            , a.oslo_position_specification as Specification
            , a.geometry as Position
            from integration_address.address_latest_items as a
            JOIN integration_streetname.streetname_latest_items as s on a.street_name_persistent_local_id = s.persistent_local_id
            WHERE a.removed = false and a.oslo_status in ('InGebruik', 'Voorgesteld')
            ;";

            // https://www.npgsql.org/doc/release-notes/7.0.html#managing-type-mappings-at-the-connection-level-is-no-longer-supported
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(_connectionString);
            dataSourceBuilder.UseNetTopologySuite();
            using var dataSource = dataSourceBuilder.Build();
            using var connection = dataSource.CreateConnection();

            connection.Open();

            _addresses = connection.Query<Address>(sql).ToList();

            return _addresses;
        }
    }

    public class Address
    {
        public Address(
            int addressPersistentLocalId,
            int streetNamePersistentLocalId,
            int? parentPersistentLocalId,
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
            ParentPersistentLocalId = parentPersistentLocalId;
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
        public int? ParentPersistentLocalId { get; set; }
        public bool IsHouseNumber => !IsBoxNumber;
        public bool IsBoxNumber => ParentPersistentLocalId.HasValue;
        public string PostalCode { get; set; }
        public string HouseNumber { get; set; }
        public string? BoxNumber { get; set; }
        public string Status { get; set; }
        public string Method { get; set; }
        public string Specification { get; set; }
        public Point Position { get; set; }
    }
}
