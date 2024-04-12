create function integration_parcel.addresses_not_within_parcel(p_nis_code text)
    returns TABLE(address_persistent_local_id integer, capakey character varying)
    language plpgsql
as
$$
begin
    CREATE TEMP TABLE temp_address AS
    select address.persistent_local_id, address.geometry
    from integration_address.address_latest_items as address
    inner join integration_streetname.streetname_latest_items as street_name
        on address.street_name_persistent_local_id = street_name.persistent_local_id and street_name.nis_code = p_nis_code
    ;

    RETURN QUERY
    select temp_address.persistent_local_id as address_persistent_local_id, parcel.capakey
    from integration_parcel.parcel_latest_item_addresses as parcel_address
    inner join temp_address on parcel_address.address_persistent_local_id = temp_address.persistent_local_id
    inner join integration_parcel.parcel_latest_items as parcel
        on parcel_address.parcel_id = parcel.parcel_id
        and NOT ST_WITHIN(temp_address.geometry, parcel.geometry)
    ;

    DROP TABLE temp_address;

end;
$$;
