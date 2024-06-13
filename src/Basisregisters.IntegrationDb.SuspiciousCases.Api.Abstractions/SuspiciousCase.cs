namespace Basisregisters.IntegrationDb.SuspiciousCases.Api.Abstractions
{
    using System.Collections.Generic;

    public class SuspiciousCase
    {
        public static readonly IDictionary<SuspiciousCasesType, SuspiciousCase> AllCases = new Dictionary<SuspiciousCasesType, SuspiciousCase>
        {
            {
                SuspiciousCasesType.CurrentStreetNameWithoutLinkedRoadSegment,
                new SuspiciousCase(Category.StreetName, "Straatnamen 'in gebruik' zonder koppeling met wegsegment", Severity.Incorrect)
            },
            {
                SuspiciousCasesType.CurrentAddressWithoutLinkedParcelsOrBuildingUnits,
                new SuspiciousCase(Category.Address, "Adressen 'in gebruik' zonder koppeling aan perceel of gebouweenheid", Severity.Incorrect)
            },
            {
                SuspiciousCasesType.ActiveAddressLinkedToMultipleBuildingUnits,
                new SuspiciousCase(Category.Address, "Actieve adressen gekoppeld aan meerdere gebouweenheden", Severity.Suspicious)
            },
            {
                SuspiciousCasesType.StreetNameLongerThanTwoYearsProposed,
                new SuspiciousCase(Category.StreetName, "Straatnamen langer dan 2 jaar 'voorgesteld'", Severity.Suspicious)
            },
            {
                SuspiciousCasesType.ActiveAddressOutsideOfMunicipalityBounds,
                new SuspiciousCase(Category.Address, "Adressen die buiten de grenzen van de gemeente vallen", Severity.Incorrect)
            },
            {
                SuspiciousCasesType.AddressLongerThanTwoYearsProposed,
                new SuspiciousCase(Category.Address, "Adressen die langer dan 2 jaar bestaan en nog de status 'voorgesteld' hebben", Severity.Suspicious)
            },
            {
                SuspiciousCasesType.CurrentAddressesWithSpecificationDerivedFromBuildingUnitWithoutLinkedBuildingUnit,
                new SuspiciousCase(Category.Address, "Adressen met specificatie 'gebouweenheid ' en status 'inGebruik' zonder koppeling met gebouweenheid", Severity.Suspicious)
            },
            {
                SuspiciousCasesType.CurrentAddressLinkedWithBuildingUnitButNotWithParcel,
                new SuspiciousCase(Category.Address, "Actuele gebouweenheden met koppeling adres 'in gebruik' maar adres is niet gekoppeld aan onderliggend perceel", Severity.Suspicious)
            },
            {
                SuspiciousCasesType.ProposedAddressWithoutLinkedParcelOrBuildingUnit,
                new SuspiciousCase(Category.Address, "Adressen 'voorgesteld' zonder koppeling aan perceel of gebouweenheid", Severity.Suspicious)
            },
            {
                SuspiciousCasesType.BuildingLongerThanTwoYearsPlanned,
                new SuspiciousCase(Category.Building, "Gebouwen die langer dan 2 jaar bestaan en nog steeds de status 'gepland' hebben", Severity.Suspicious)
            },
            // {
            //     SuspiciousCasesType.ActiveBuildingUnitWithoutAddress,
            //     new SuspiciousCase(Category.BuildingUnit, "Actuele Gebouweenheden zonder adres", Severity.Suspicious)
            // },
            {
                SuspiciousCasesType.BuildingUnitLongerThanTwoYearsPlanned,
                new SuspiciousCase(Category.BuildingUnit, "Gebouweenheden die langer dan 2 jaar bestaan en nog steeds de status 'gepland' hebben", Severity.Suspicious)
            },
            {
                SuspiciousCasesType.RoadSegmentLongerThanTwoYearsWithPermit,
                new SuspiciousCase(Category.RoadSegment, "Wegsegmenten die langer dan 2 jaar bestaan en nog steeds de status 'voorgesteld' hebben", Severity.Suspicious)
            },
            {
                SuspiciousCasesType.MeasuredRoadSegmentWithNoOrSingleLinkedStreetName,
                new SuspiciousCase(Category.RoadSegment, "Ingemeten wegsegementen die slechts 1 of geen koppeling hebben met straatnaam", Severity.Suspicious)
            },
            // {
            //     SuspiciousCasesType.ActiveBuildingUnitLinkedToMultipleAddresses,
            //     new SuspiciousCase(Category.BuildingUnit, "Gebouweenheden met status 'gepland' of 'gerealiseerd' die gekoppeld zijn aan meerdere adressen", Severity.Improvable)
            // },
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
