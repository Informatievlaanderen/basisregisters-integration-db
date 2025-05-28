namespace Basisregisters.IntegrationDb.SuspiciousCases.Views.Internal
{
    using Infrastructure;

    public sealed class InactiveParcelLinkedToAddress
    {
        public const string ViewName = "view_inactive_parcel_unit_linked_to_address";

        public const string Create = $@"
            DROP VIEW IF EXISTS {Schema.SuspiciousCases}.{ViewName};
            CREATE VIEW {Schema.SuspiciousCases}.{ViewName} AS
            SELECT
                p.capakey
                , p.status
                , p.is_removed
                , a.persistent_local_id as address_persistent_local_id
            FROM {SchemaLatestItems.ParcelAddresses} as pa
            JOIN {SchemaLatestItems.Parcel} as p
                on pa.parcel_id = p.parcel_id
                    and (p.status in ('Retired') or p.is_removed = true)
            JOIN {SchemaLatestItems.Address} as a
                on pa.address_persistent_local_id = a.persistent_local_id
            ORDER BY p.capakey
            ;";
    }
}
