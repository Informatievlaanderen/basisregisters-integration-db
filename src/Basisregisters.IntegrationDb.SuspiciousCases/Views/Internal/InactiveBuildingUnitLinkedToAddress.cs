namespace Basisregisters.IntegrationDb.SuspiciousCases.Views.Internal
{
    using Infrastructure;

    public sealed class InactiveBuildingUnitLinkedToAddress
    {
        public const string ViewName = "view_inactive_building_unit_linked_to_address";

        public const string Create = $@"
            CREATE VIEW {Schema.SuspiciousCases}.{ViewName} AS
            SELECT
                bua.building_unit_persistent_local_id
                , bu.status
                , bu.is_removed
                , bua.address_persistent_local_id as address_persistent_local_id
            FROM {SchemaLatestItems.BuildingUnitAddresses} as bua
            JOIN {SchemaLatestItems.BuildingUnit} as bu
                on bua.building_unit_persistent_local_id = bu.building_unit_persistent_local_id
                    and (bu.status in ('NotRealized','Retired') or bu.is_removed = true)
            ORDER BY bu.building_persistent_local_id, bu.building_unit_persistent_local_id
            ;";
    }
}
