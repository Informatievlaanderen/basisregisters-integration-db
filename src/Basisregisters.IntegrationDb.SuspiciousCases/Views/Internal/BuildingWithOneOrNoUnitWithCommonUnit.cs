namespace Basisregisters.IntegrationDb.SuspiciousCases.Views.Internal
{
    using Infrastructure;

    public sealed class BuildingWithOneOrNoUnitWithCommonUnit
    {
        public const string ViewName = "view_building_with_one_or_no_unit_with_common_unit";

        public const string Create = $@"
            CREATE VIEW {Schema.SuspiciousCases}.{ViewName} AS
            SELECT b.building_persistent_local_id
            FROM integration_building.building_latest_items b
            JOIN integration_building.building_unit_latest_items bu ON bu.building_persistent_local_id = b.building_persistent_local_id
	            AND bu.status in ('Planned', 'Realized') AND bu.is_removed = FALSE AND bu.""function"" <> 'Common'
            WHERE
	            b.is_removed = FALSE
            GROUP BY b.building_persistent_local_id
            HAVING count(*) <=1
            INTERSECT
            SELECT b.building_persistent_local_id
            FROM integration_building.building_latest_items b
            JOIN integration_building.building_unit_latest_items bu ON bu.building_persistent_local_id = b.building_persistent_local_id
	            AND bu.status in ('Planned', 'Realized') AND bu.is_removed = FALSE AND bu.""function"" = 'Common'
            WHERE
	            b.is_removed = FALSE
            GROUP BY b.building_persistent_local_id
            ;";
    }
}
