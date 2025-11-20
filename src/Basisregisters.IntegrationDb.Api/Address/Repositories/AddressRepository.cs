namespace Basisregisters.IntegrationDb.Api.Address.Repositories;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Npgsql;

public sealed record AddressWithGeometry(int PersistentLocalId, string Geometry);

public class AddressRepository
{
    private readonly string _connectionString;

    public AddressRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<ICollection<AddressWithGeometry>> GetAddressesToCorrectPosition(ICollection<int>? addressPersistentLocalIds)
    {
        var sql = """
                  select adr.persistent_local_id as PersistentLocalId, st_astext(bldu.geometry) as geometry
                  from integration_address.address_latest_items adr
                  --Adressen met koppeling naar een gebouweenheid
                  inner join integration_building.building_unit_addresses bldua
                  	on adr.persistent_local_id = bldua.address_persistent_local_id
                  --Voeg informatie van eenheden toe
                  inner join integration_building.building_unit_latest_items bldu
                  	on bldu.building_unit_persistent_local_id = bldua.building_unit_persistent_local_id
                  	and st_distance(adr.geometry, bldu.geometry) > 0.01 -- niet overlappende geometrie (+missende afronding negeren)
                  --Adres mag slechts aan één eenheid gekoppeld zijn
                  inner join --Selecteer adressen met slechts één gekoppelde gebouweenheid
                  (
                  	select address_persistent_local_id
                  	from integration_building.building_unit_addresses
                  	group by address_persistent_local_id
                  	having count(*) = 1
                  ) as adr_one_bldu
                  	on adr_one_bldu.address_persistent_local_id = adr.persistent_local_id
                  --Voeg informatie van gebouwen toe
                  inner join integration_building.building_latest_items bld
                  	on bld.building_persistent_local_id = bldu.building_persistent_local_id
                  where adr.removed = false
                  	and adr.oslo_position_method = 'AfgeleidVanObject'
                  	and adr.oslo_position_specification = 'Gebouweenheid'
                  	and adr.oslo_status in ('InGebruik', 'Voorgesteld')
                  	and bldu.is_removed = false
                  	and bldu.oslo_status in ('Gepland', 'Gerealiseerd')
                  	and st_within(adr.geometry, bld.geometry)
                  """;

        if (addressPersistentLocalIds?.Count > 0)
        {
            sql += $" AND adr.persistent_local_id IN ({string.Join(",", addressPersistentLocalIds)})";
        }
        else
        {
            // Minstens 2 dagen oud
            sql += " AND adr.version_timestamp < (now() - interval '2 days') AND bldu.version_timestamp < (now() - interval '2 days')";
        }

        await using var connection = new NpgsqlConnection(_connectionString);

        var addresses = await connection.QueryAsync<AddressWithGeometry>(sql, commandTimeout: 60 * 15);

        return addresses.ToList();
    }
}
