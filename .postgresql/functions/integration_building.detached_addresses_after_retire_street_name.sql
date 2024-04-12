create function integration_building.detached_addresses_after_retire_street_name(p_street_name_id integer)
    returns TABLE(address_persistent_local_id integer, building_unit_persistent_local_id integer)
    language plpgsql
as
$$
declare
    timestamp TIMESTAMPTZ;

begin
    select version_timestamp into timestamp
    from integration_streetname.streetname_versions
    where persistent_local_id = p_street_name_id and type = 'StreetNameWasRetiredV2'
    ;

    CREATE TEMP TABLE temp_addresses AS
    select address_version.persistent_local_id
    from integration_address.address_versions as address_version
    where
        address_version.street_name_persistent_local_id = p_street_name_id
        and address_version.version_timestamp >= timestamp
        and (address_version.type = 'AddressWasRetiredBecauseStreetNameWasRetired' or address_version.type = 'AddressWasRejectedBecauseStreetNameWasRetired')
    ;

    -- min(position) wordt gebruikt indien de gebouweenheid aan meerdere adressen is gekoppeld
    CREATE TEMP TABLE temp_building_unit_positions AS
    select building_unit_version.building_unit_persistent_local_id, min(building_unit_version.position) - 1 AS position
    from integration_building.building_unit_versions as building_unit_version
    where
        building_unit_version.version_timestamp >= timestamp and building_unit_version.version_timestamp <= timestamp + INTERVAL '1 hour'
        and
        (
          building_unit_version.type = 'BuildingUnitAddressWasDetachedBecauseAddressWasRejected'
          or building_unit_version.type = 'BuildingUnitAddressWasDetachedBecauseAddressWasRetired'
        )
    group by building_unit_version.building_unit_persistent_local_id
    ;

    -- filter de gebouweenheden waarvoor er een koppeling met een adres bestond op de gevonden posities
    RETURN QUERY
    select building_unit_address.address_persistent_local_id, building_unit_address.building_unit_persistent_local_id
    from integration_building.building_unit_address_versions as building_unit_address
    inner join temp_building_unit_positions
        on building_unit_address.building_unit_persistent_local_id = temp_building_unit_positions.building_unit_persistent_local_id
        and building_unit_address.position = temp_building_unit_positions.position
    inner join temp_addresses
        on building_unit_address.address_persistent_local_id = temp_addresses.persistent_local_id
    ;

    DROP TABLE temp_addresses;
    DROP TABLE temp_building_unit_positions;

end;
$$;

