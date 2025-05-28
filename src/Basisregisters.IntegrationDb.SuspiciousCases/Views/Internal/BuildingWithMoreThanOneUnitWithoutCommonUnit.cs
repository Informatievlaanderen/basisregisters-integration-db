namespace Basisregisters.IntegrationDb.SuspiciousCases.Views.Internal
{
    using Infrastructure;

    public sealed class BuildingWithMoreThanOneUnitWithoutCommonUnit
    {
        public const string ViewName = "view_building_with_more_than_one_unit_without_common_unit";

        public const string Create = $@"
            DROP VIEW IF EXISTS {Schema.SuspiciousCases}.{ViewName};
            CREATE VIEW {Schema.SuspiciousCases}.{ViewName} AS
            SELECT b.building_persistent_local_id
            FROM integration_building.building_latest_items b
            JOIN integration_building.building_unit_latest_items bu ON bu.building_persistent_local_id = b.building_persistent_local_id
	            AND bu.status in ('Planned', 'Realized') AND bu.is_removed = FALSE AND bu.""function"" <> 'Common'
            WHERE
	            b.is_removed = FALSE
            GROUP BY b.building_persistent_local_id
            HAVING count(*) >= 2
            EXCEPT
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
