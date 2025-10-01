namespace Basisregisters.IntegrationDb.Api.Abstractions.SuspiciousCase
{
    using System.Collections.Generic;
    using Basisregisters.IntegrationDb.SuspiciousCases;

    public class SuspiciousCase
    {
        public static readonly IDictionary<SuspiciousCasesType, SuspiciousCase> AllCases = new Dictionary<SuspiciousCasesType, SuspiciousCase>
        {
            {
                SuspiciousCasesType.CurrentStreetNameWithoutLinkedRoadSegment,
                new SuspiciousCase(Category.StreetName, "Straatnaam \"in gebruik\" zonder koppeling met wegsegment", Severity.Incorrect)
            },
            {
                SuspiciousCasesType.CurrentAddressWithoutLinkedParcelsOrBuildingUnits,
                new SuspiciousCase(Category.Address, "Adres \"in gebruik\" zonder koppeling met perceel of gebouweenheid", Severity.Incorrect)
            },
            {
                SuspiciousCasesType.ActiveAddressLinkedToMultipleBuildingUnits,
                new SuspiciousCase(Category.Address, "Actief adres gekoppeld aan meerdere actieve gebouweenheden", Severity.Suspicious)
            },
            {
                SuspiciousCasesType.StreetNameLongerThanTwoYearsProposed,
                new SuspiciousCase(Category.StreetName, "Straatnaam bestaat langer dan 2 jaar en heeft nog steeds de status \"voorgesteld\"", Severity.Suspicious)
            },
            {
                SuspiciousCasesType.ActiveAddressOutsideOfMunicipalityBounds,
                new SuspiciousCase(Category.Address, "Actief adres dat buiten de grenzen van de gemeente valt", Severity.Incorrect)
            },
            {
                SuspiciousCasesType.AddressLongerThanTwoYearsProposed,
                new SuspiciousCase(Category.Address, "Adres bestaat langer dan 2 jaar en heeft nog de status \"voorgesteld\"", Severity.Suspicious)
            },
            {
                SuspiciousCasesType.CurrentAddressesWithSpecificationDerivedFromBuildingUnitWithoutLinkedBuildingUnit,
                new SuspiciousCase(Category.Address, "Adres \"in gebruik\" met specificatie “afgeleid van gebouweenheid” zonder koppeling met gebouweenheid", Severity.Suspicious)
            },
            {
                SuspiciousCasesType.CurrentAddressLinkedWithBuildingUnitButNotWithParcel,
                new SuspiciousCase(Category.Address, "Adres \"in gebruik\" met koppeling naar een actieve gebouweenheid, maar zonder koppeling naar het onderliggend perceel", Severity.Suspicious)
            },
            {
                SuspiciousCasesType.ProposedAddressWithoutLinkedParcelOrBuildingUnit,
                new SuspiciousCase(Category.Address, "Adres \"voorgesteld\" zonder koppeling met perceel of gebouweenheid", Severity.Suspicious)
            },
            {
                SuspiciousCasesType.BuildingLongerThanTwoYearsPlanned,
                new SuspiciousCase(Category.Building, "Gebouw bestaat langer dan 2 jaar en heeft nog steeds de status \"gepland\"", Severity.Suspicious)
            },
            // {
            //     SuspiciousCasesType.ActiveBuildingUnitWithoutAddress,
            //     new SuspiciousCase(Category.BuildingUnit, "Actuele Gebouweenheden zonder adres", Severity.Suspicious)
            // },
            {
                SuspiciousCasesType.BuildingUnitLongerThanTwoYearsPlanned,
                new SuspiciousCase(Category.BuildingUnit, "Gebouweenheid bestaat langer dan 2 jaar en heeft nog steeds de status \"gepland\"", Severity.Suspicious)
            },
            {
                SuspiciousCasesType.RoadSegmentLongerThanTwoYearsWithPermit,
                new SuspiciousCase(Category.RoadSegment, "Wegsegment bestaat langer dan 2 jaar en heeft nog steeds de status \"voorgesteld\"", Severity.Suspicious)
            },
            {
                SuspiciousCasesType.RoadSegmentWithSingleLinkedStreetName,
                new SuspiciousCase(Category.RoadSegment, "Wegsegment met slechts 1 straatnaamkoppeling", Severity.Suspicious)
            },
            // {
            //     SuspiciousCasesType.ActiveBuildingUnitLinkedToMultipleAddresses,
            //     new SuspiciousCase(Category.BuildingUnit, "Gebouweenheden met status 'gepland' of 'gerealiseerd' die gekoppeld zijn aan meerdere adressen", Severity.Improvable)
            // },
            {
                SuspiciousCasesType.CurrentAddressLinkedToProposedStreetName,
                new SuspiciousCase(Category.Address, "Adres \"in gebruik\" met koppeling naar een straatnaam met de status \"voorgesteld\"", Severity.Suspicious)
            },
            {
                SuspiciousCasesType.RoadSegmentLinkedToRetiredStreetName,
                new SuspiciousCase(Category.RoadSegment, "Straatnaam \"afgekeurd\" of \"gehistoreerd\" met koppeling met een wegsegment", Severity.Incorrect)
            }
        };

        public Category Category { get; }
        public string Description { get; }
        public Severity Severity { get; }

        private SuspiciousCase(Category category, string description, Severity severity)
        {
            Category = category;
            Description = description;
            Severity = severity;
        }
    }
}
