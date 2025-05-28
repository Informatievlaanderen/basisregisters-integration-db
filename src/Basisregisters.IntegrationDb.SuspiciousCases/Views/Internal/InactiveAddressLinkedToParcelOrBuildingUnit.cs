namespace Basisregisters.IntegrationDb.SuspiciousCases.Views.Internal
{
    using Infrastructure;

    public sealed class InactiveAddressLinkedToParcelOrBuildingUnit
    {
        public const string ViewName = "view_inactive_address_linked_to_parcel_or_building_unit";

        public const string Create = $@"
            DROP VIEW IF EXISTS {Schema.SuspiciousCases}.{ViewName};
            CREATE VIEW {Schema.SuspiciousCases}.{ViewName} AS
            SELECT *
            FROM
            (
                SELECT
                    a.persistent_local_id as address_persistent_local_id
                    , a.status
                    , a.removed
                    , cast(bua.building_unit_persistent_local_id as text) as linked_id
                    , 'buildingUnit' as type
                FROM {SchemaLatestItems.BuildingUnitAddresses} as bua
                JOIN {SchemaLatestItems.Address} as a
                    on bua.address_persistent_local_id = a.persistent_local_id
                       and a.status in (3, 4)
                UNION
                SELECT
                    a.persistent_local_id as address_persistent_local_id
                    , a.status
                    , a.removed
                    , pa.capakey as linked_id
                    , 'parcel' as type
                FROM {SchemaLatestItems.ParcelAddresses} as pa
                JOIN {SchemaLatestItems.Address} as a
                    on pa.address_persistent_local_id = a.persistent_local_id
                       and a.status in (3, 4)
            ) as u
            ORDER BY u.address_persistent_local_id
            ;";
    }
}
