namespace Basisregisters.IntegrationDb.SuspiciousCases
{
    public enum SuspiciousCasesType
    {
        CurrentStreetNameWithoutLinkedRoadSegment = 1,
        CurrentAddressWithoutLinkedParcelsOrBuildingUnits = 2,
        ActiveAddressLinkedToMultipleBuildingUnits = 3,
        StreetNameLongerThanTwoYearsProposed = 4,
        ActiveAddressOutsideOfMunicipalityBounds = 5,
        AddressLongerThanTwoYearsProposed = 6,
        CurrentAddressesWithSpecificationDerivedFromBuildingUnitWithoutLinkedBuildingUnit = 7,
        CurrentAddressLinkedWithBuildingUnitButNotWithParcel = 8,
        ProposedAddressWithoutLinkedParcelOrBuildingUnit = 9,
        BuildingLongerThanTwoYearsPlanned = 10,
        // ActiveBuildingUnitWithoutAddress = 11,
        BuildingUnitLongerThanTwoYearsPlanned = 12,
        RoadSegmentsLongerThanTwoYearsProposed = 13,
        StreetNameWithOnlyOneRoadSegmentToOnlyOneSide = 14,

        // ActiveBuildingUnitLinkedToMultipleAddresses = 15,
    }
}
