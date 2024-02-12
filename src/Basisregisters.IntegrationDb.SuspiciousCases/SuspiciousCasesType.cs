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
        AddressesWithBuildingUnitSpecificationOutsideLinkedActiveBuildingUnit = 11,
        ActiveBuildingUnitsWithoutAddress = 12,
        ActiveBuildingUnitsLinkedToMultipleAddresses = 13,
        AddressesLinkedToMultipleBuildingUnits = 14
    }
}
