namespace Basisregisters.IntegrationDb.SuspiciousCases.Api
{
    using System.Collections.Generic;
    using Schema.Models;

    public class SuspiciousCase
    {
        public static IDictionary<SuspiciousCasesType, SuspiciousCase> AllCases = new Dictionary<SuspiciousCasesType, SuspiciousCase>
        {
            {
                SuspiciousCasesType.CurrentAddressWithoutLinkedParcelsOrBuildingUnits,
                new SuspiciousCase(Category.Address, "Adressen 'in gebruik' zonder koppeling aan perceel of gebouweenheid", Severity.Incorrect)
            },
            {
                SuspiciousCasesType.ProposedAddressWithoutLinkedParcelsOrBuildingUnits,
                new SuspiciousCase(Category.Address, "Adressen 'voorgesteld' zonder koppeling aan perceel of gebouweenheid", Severity.Suspicious)
            },
            {
                SuspiciousCasesType.AddressesOutsideOfMunicipalityBoundaries,
                new SuspiciousCase(Category.Address, "Adressen die buiten de grenzen van de gemeente vallen", Severity.Incorrect)
            },
            {
                SuspiciousCasesType.CurrentStreetNamesWithoutRoadSegment,
                new SuspiciousCase(Category.StreetName, "Straatnamen 'in gebruik' zonder koppeling met wegverbinding", Severity.Incorrect)
            },
            {
                SuspiciousCasesType.StreetNamesLongerThanTwoYearsProposed,
                new SuspiciousCase(Category.StreetName, "Straatnamen langer dan 2 jaar 'voorgesteld'", Severity.Improveable)
            },
            {
                SuspiciousCasesType.AddressesLongerThanTwoYearsProposed,
                new SuspiciousCase(Category.Address, "Adressen bestaat langer dan 2 jaar en heeft nog de status 'voorgesteld'", Severity.Improveable)
            },
            {
                SuspiciousCasesType.RoadSegmentsLongerThanTwoYearsProposed,
                new SuspiciousCase(Category.RoadSegment, "Wegverbindingen bestaat langer dan 2 jaar en heeft nog de status 'voorgesteld'", Severity.Improveable)
            },
            {
                SuspiciousCasesType.BuildingLongerThanTwoYearsPlanned,
                new SuspiciousCase(Category.Building, "Gebouw bestaat langer dan 2 jaar en heeft nog steeds de status 'gepland'", Severity.Improveable)
            },
            {
                SuspiciousCasesType.BuildingUnitLongerThanTwoYearsPlanned,
                new SuspiciousCase(Category.Buildingunit, "Gebouweenheden bestaat langer dan 2 jaar en heeft nog de status 'gepland'", Severity.Improveable)
            },
            {
                SuspiciousCasesType.StreetNameWithOnlyOneRoadSegmentToOnlyOneSide,
                new SuspiciousCase(Category.RoadSegment, "Straatnaam waarbij enkel een wegverbindingen gekoppeld aan slechts 1 kant", Severity.Improveable)
            },
            {
                SuspiciousCasesType.AddressesAppointedByAdministratorOutsideLinkedBuilding,
                new SuspiciousCase(Category.Address, "Adressen met herkomst 'AangeduidDoorBeheerder' buiten het gekoppelde gebouw", Severity.Improveable)
            },
            {
                SuspiciousCasesType.AddressesWithBuildingUnitSpecificationOutsideLinkedActiveBuildingUnit,
                new SuspiciousCase(Category.Address, "Adressen met specificatie 'gebouweenheid ' en status 'inGebruik' zonder koppeling met gebouweenheid", Severity.Improveable)
            },
            {
                SuspiciousCasesType.BuildingUnitsWithoutAddress,
                new SuspiciousCase(Category.Buildingunit, "Gebouweenheden met status 'gepland' of 'gerealiseerd' die niet gekoppeld zijn aan een adres", Severity.Improveable)
            },
            {
                SuspiciousCasesType.BuildingUnitsLinkedToMultipleAddresses,
                new SuspiciousCase(Category.Buildingunit, "Gebouweenheden met status 'gepland' of 'gerealiseerd' die gekoppeld zijn aan meerdere adressen", Severity.Improveable)
            },
            {
                SuspiciousCasesType.AddressesLinkedToMultipleBuildingUnits,
                new SuspiciousCase(Category.Address, "Adressen met status 'voorgesteld' of 'inGebruik' die gekoppeld zijn aan meerdere gebouweenheden", Severity.Improveable)
            }
        };

        public Category Category { get; }
        public string Description { get; }
        public Severity Severity { get; }

        // Action

        private SuspiciousCase(Category category, string description, Severity severity)
        {
            Category = category;
            Description = description;
            Severity = severity;
        }
    }
}
