namespace Basisregisters.IntegrationDb.Api.Address.Repositories;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Npgsql;

/// <param name="Geometry">Geometry of the building unit, the new position of the address.</param>
/// <param name="Srid">SRID of <paramref name="Geometry"/> (Lambert72 31370 or Lambert08 3812).</param>
/// <param name="AddressSrid">SRID of the current address position; must equal <paramref name="Srid"/>.</param>
public sealed record AddressWithGeometry(int PersistentLocalId, string Geometry, int Srid, int AddressSrid);

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
                  select adr.persistent_local_id as PersistentLocalId, st_astext(bldu.geometry) as geometry, st_srid(bldu.geometry) as srid, st_srid(adr.geometry) as addresssrid
                  from integration_address.address_latest_items_v2 adr
                  --Adressen met koppeling naar een gebouweenheid
                  inner join integration_building.building_unit_addresses bldua
                  	on adr.persistent_local_id = bldua.address_persistent_local_id
                  --Voeg informatie van eenheden toe
                  inner join integration_building.building_unit_latest_items bldu
                  	on bldu.building_unit_persistent_local_id = bldua.building_unit_persistent_local_id
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
                  	--Adres en gebouweenheid moeten in hetzelfde referentiesysteem staan. Anders niet vergelijken (gemengde SRID's geven een fout)
                  	--maar de rij teruggeven, zodat ze hieronder met een duidelijke fout gemeld wordt. CASE garandeert de evaluatievolgorde.
                  	and case
                  		when adr.geometry is null then false
                  		when st_srid(adr.geometry) = st_srid(bldu.geometry)
                  			then st_distance(adr.geometry, bldu.geometry) > 0.01 -- niet overlappende geometrie (+missende afronding negeren)
                  				and st_within(adr.geometry, bld.geometry)
                  		else true
                  	end
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

        var addresses = (await connection.QueryAsync<AddressWithGeometry>(sql, commandTimeout: 60 * 15)).ToList();

        // An address and its building unit should always be in the same SRID. We don't convert between
        // Lambert72 and Lambert08 here: ST_Transform doesn't transform the same way the registries do.
        var sridMismatches = addresses.Where(x => x.AddressSrid != x.Srid).ToList();
        if (sridMismatches.Count > 0)
        {
            throw new InvalidOperationException(
                "Address and building unit are in a different SRID, no positions were corrected: "
                + string.Join(", ", sridMismatches.Select(x => $"{x.PersistentLocalId} (address {x.AddressSrid}, building unit {x.Srid})")));
        }

        return addresses;
    }
}
