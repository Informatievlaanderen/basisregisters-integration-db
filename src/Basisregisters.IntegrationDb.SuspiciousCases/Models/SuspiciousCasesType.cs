namespace Basisregisters.IntegrationDb.Schema.Models
{
    public enum SuspiciousCasesType
    {
        CurrentAddressWithoutLinkedParcelsOrBuildingUnits = 1,
        ProposedAddressWithoutLinkedParcelsOrBuildingUnits = 2,
        AddressesOutsideOfMunicipalityBoundaries = 3,
        CurrentStreetNamesWithoutRoadSegment = 4,
        StreetNamesLongerThanTwoYearsProposed = 5,
        AddressesLongerThanTwoYearsProposed = 6,
        RoadSegmentsLongerThanTwoYearsProposed = 7,
        BuildingLongerThanTwoYearsPlanned = 8,
        BuildingUnitLongerThanTwoYearsPlanned = 9,
        StreetNameWithOnlyOneRoadSegmentToOnlyOneSide = 10,
        AddressesAppointedByAdministratorOutsideLinkedBuilding = 11,
        AddressesWithBuildingUnitSpecificationOutsideLinkedActiveBuildingUnit = 12,
        BuildingUnitsWithoutAddress = 13,
        BuildingUnitsLinkedToMultipleAddresses = 14,
        AddressesLinkedToMultipleBuildingUnits = 15
    }
}
