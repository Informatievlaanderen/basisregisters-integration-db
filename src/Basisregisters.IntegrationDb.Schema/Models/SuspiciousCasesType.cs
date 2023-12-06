namespace Basisregisters.IntegrationDb.Schema.Models
{
    public enum SuspiciousCasesType
    {
        CurrentHouseNumbersWithoutLinkedParcel = 1,
        HouseNumbersWithDifferentPostalCode = 2,
        HouseNumbersWithoutPostalCode = 3,
        AddressesOutsideOfMunicipality = 4,
        CurrentStreetNamesWithoutRoadSegment = 5,
        HouseNumbersWithMultipleLinks = 6, // TODO: verify name with query
        SimilarStreetNameNames = 7,
        HouseNumbersNotSituatedBetweenPreviousAndNextHouseNumber = 8,
        ParcelsLinkedToMultipleHouseNumbers = 9
    }
}
