namespace Basisregisters.IntegrationDb.SuspiciousCases.Infrastructure
{
    public static class SchemaLatestItems
    {
        public const string StreetName = "integration_streetname.streetname_latest_items";
        public const string Address = "integration_address.address_latest_items";
        public const string Building = "integration_building.building_latest_items";
        public const string BuildingUnit = "integration_building.building_unit_latest_items";
        public const string BuildingUnitAddresses = "integration_building.building_unit_addresses";
        public const string Parcel = "integration_parcel.parcel_latest_items";
        public const string ParcelAddresses = "integration_parcel.parcel_latest_item_addresses";
        public const string Postal = "integration_postal.postal_latest_items";
        public const string RoadSegment = "integration_road.road_segment_latest_items";
        public const string Municipality = "integration_municipality.municipality_latest_items";
        public const string MunicipalityGeometries = "integration_municipality.municipality_geometries";
    }

    public static class Schema
    {
        public const string SuspiciousCases = "integration_suspicious_cases";
        public const string FullAddress = "integration_suspicious_cases.get_full_address";
    }
}
