﻿namespace Basisregisters.IntegrationDb.SuspiciousCases.Views.Internal
{
    using Infrastructure;

    public sealed class ActiveBuildingUnitOutsideBuilding
    {
        public const string ViewName = "view_active_building_unit_outside_building";

        public const string Create = $@"
            DROP VIEW IF EXISTS {Schema.SuspiciousCases}.{ViewName};
            CREATE VIEW {Schema.SuspiciousCases}.{ViewName} AS
            SELECT
                bu.building_persistent_local_id,
                bu.building_unit_persistent_local_id
            FROM {SchemaLatestItems.BuildingUnit} as bu
            JOIN {SchemaLatestItems.Building} as b on bu.building_persistent_local_id = b.building_persistent_local_id
            WHERE
                bu.is_removed = false
                AND bu.status in ('Planned', 'Realized')
                AND ST_Within(bu.geometry, b.geometry) IS FALSE
            ORDER BY bu.building_persistent_local_id, bu.building_unit_persistent_local_id
            ;";
    }
}
