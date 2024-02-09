namespace Basisregisters.IntegrationDb.SuspiciousCases
{
    public enum SuspiciousCasesType
    {
        CurrentAddressWithoutLinkedParcelsOrBuildingUnits = 1,
        ProposedAddressWithoutLinkedParcelsOrBuildingUnits = 2,
        CurrentAddressesOutsideOfMunicipalityBounds = 3,
        CurrentStreetNamesWithoutRoadSegment = 4,
        StreetNamesLongerThanTwoYearsProposed = 5,
        AddressesLongerThanTwoYearsProposed = 6,
        RoadSegmentsLongerThanTwoYearsProposed = 7,
        BuildingLongerThanTwoYearsPlanned = 8,
        BuildingUnitLongerThanTwoYearsPlanned = 9,
        StreetNameWithOnlyOneRoadSegmentToOnlyOneSide = 10,
        AddressesAppointedByAdministratorOutsideLinkedBuilding = 11,
        AddressesWithBuildingUnitSpecificationOutsideLinkedActiveBuildingUnit = 12,
        ActiveBuildingUnitsWithoutAddress = 13,
        ActiveBuildingUnitsLinkedToMultipleAddresses = 14,
        AddressesLinkedToMultipleBuildingUnits = 15
    }
}
