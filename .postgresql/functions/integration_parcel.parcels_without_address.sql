create function integration_parcel.parcels_without_address(p_nis_code text)
    returns TABLE(capakey character varying)
    language plpgsql
as
$$
declare
    municipality_geometry geometry;

begin
    select geometry into municipality_geometry
    from integration_municipality.municipality_geometries
    where nis_code = p_nis_code
    ;

    RETURN QUERY
    select parcel.capakey
    from integration_parcel.parcel_latest_items as parcel
    left outer join integration_parcel.parcel_latest_item_addresses as parcel_address
        on parcel.parcel_id = parcel_address.parcel_id
    where parcel_address.parcel_id is null
        and  ST_INTERSECTS(municipality_geometry, parcel.geometry)
    ;

end;
$$;
