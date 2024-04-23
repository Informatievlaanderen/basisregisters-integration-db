namespace Basisregisters.IntegrationDb.Bosa.Repositories
{
    using System.Collections.Generic;
    using Basisregisters.IntegrationDB.Bosa.Infrastructure;
    using Dapper;
    using Model.Database;
    using Npgsql;

    public interface IAddressRepository
    {
        IEnumerable<Address> GetFlemish();
    }

    public class AddressRepository : IAddressRepository
    {
        private readonly string _connectionString;

        public AddressRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IEnumerable<Address> GetFlemish()
        {
            const string sql = @$"
select
    a.namespace as Namespace
	, a.persistent_local_id as AddressPersistentLocalId
	, a.street_name_persistent_local_id as StreetNamePersistentLocalId
	, a.postal_code as PostalCode
	, a.version_timestamp as VersionTimestamp
	, ac.version_timestamp as CrabVersionTimestamp
    , (select av.created_on_timestamp
       from integration_address.address_versions av
       where av.persistent_local_id = a.persistent_local_id
       order by created_on_timestamp
       limit 1) as CreatedOn
    , ST_X(a.geometry) as X
    , ST_Y(a.geometry) as Y
    , ST_SRID(a.geometry) as SrId
    , a.position_method as PositionGeometryMethod
    , a.position_specification as PositionSpecification
	, a.status as Status
	, a.house_number as HouseNumber
	, a.box_number as BoxNumber
	, a.officially_assigned as OfficiallyAssigned
from integration_address.address_latest_items a
left join {DatabaseSetup.Schema}.{DatabaseSetup.AddressCrabVersionsTable} ac on ac.address_persistent_local_id = a.persistent_local_id
where a.removed = false and a.postal_code is not null
order by a.persistent_local_id";

            using var connection = new NpgsqlConnection(_connectionString);

            var addresses = connection.Query<Address>(sql);

            return addresses;
        }
    }
}
