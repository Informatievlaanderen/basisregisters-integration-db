create function integration_parcel.detached_addresses_after_retire_street_name(p_street_name_id integer)
    returns TABLE(address_persistent_local_id integer, capakey character varying)
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

    -- min(position) wordt gebruikt indien het perceel aan meerdere adressen is gekoppeld
    CREATE TEMP TABLE temp_parcel_positions AS
    select parcel_version.capakey, min(parcel_version.position) - 1 AS position
    from integration_parcel.parcel_version as parcel_version
    where
        parcel_version.version_timestamp >= timestamp and parcel_version.version_timestamp <= timestamp + INTERVAL '1 hour'
        and
        (
          parcel_version.type = 'BuildingUnitAddressWasDetachedBecauseAddressWasRejected'
          or parcel_version.type = 'BuildingUnitAddressWasDetachedBecauseAddressWasRetired'
        )
    group by parcel_version.capakey
    ;

    -- filter de percelen waarvoor er een koppeling met een adres bestond op de gevonden posities
    RETURN QUERY
    select parcel_address.address_persistent_local_id, temp_parcel_positions.capakey
    from integration_parcel.parcel_version_addresses as parcel_address
    inner join temp_parcel_positions
        on parcel_address.capakey = temp_parcel_positions.capakey
        and parcel_address.position = temp_parcel_positions.position
    inner join temp_addresses
        on parcel_address.address_persistent_local_id = temp_addresses.persistent_local_id
    ;

    DROP TABLE temp_addresses;
    DROP TABLE temp_parcel_positions;

end;
$$;
