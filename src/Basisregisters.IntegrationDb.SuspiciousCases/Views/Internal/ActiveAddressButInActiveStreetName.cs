namespace Basisregisters.IntegrationDb.SuspiciousCases.Views.Internal
{
    using Infrastructure;

    public sealed class ActiveAddressButInActiveStreetName
    {
        public const string ViewName = "view_active_address_but_inactive_streetname";

        public const string Create = $@"
            DROP VIEW IF EXISTS {Schema.SuspiciousCases}.{ViewName};
            CREATE VIEW {Schema.SuspiciousCases}.{ViewName} AS
            SELECT
                s.persistent_local_id as streetname_persistent_local_id
                , a.persistent_local_id as address_persistent_local_id
                , s.status as streetname_status
                , a.status as address_status
                , s.is_removed as streetname_is_removed
            FROM {SchemaLatestItems.Address} as a
            JOIN {SchemaLatestItems.StreetName} as s
                on a.street_name_persistent_local_id = s.persistent_local_id
                    and (s.status in (2, 3) or s.is_removed = true)
            WHERE
                a.status in (1, 2)
                and a.removed = false
            ;";
    }
}
