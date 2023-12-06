namespace Basisregisters.IntegrationDb.SuspiciousCases.Api
{
    using System.Collections.Generic;
    using Schema.Models;

    public class SuspiciousCase
    {
        public static IDictionary<SuspiciousCasesType, SuspiciousCase> AllCases = new Dictionary<SuspiciousCasesType, SuspiciousCase>
        {
            {
                SuspiciousCasesType.CurrentHouseNumbersWithoutLinkedParcel,
                new SuspiciousCase(Category.Address, "Huisnummers \"in gebruik\" zonder koppeling met perceel", Severity.Incorrect)
            },
            {
                SuspiciousCasesType.HouseNumbersWithDifferentPostalCode,
                new SuspiciousCase(Category.Address, "Huisnummers met een afwijkende postcode", Severity.Incorrect)
            },
            {
                SuspiciousCasesType.HouseNumbersWithoutPostalCode,
                new SuspiciousCase(Category.Address, "Huisnummers zonder postcode", Severity.Incorrect)
            },
            {
                SuspiciousCasesType.AddressesOutsideOfMunicipality,
                new SuspiciousCase(Category.Address, "Adressen die buiten de grenzen van de gemeente vallen", Severity.Incorrect)
            },
            {
                SuspiciousCasesType.CurrentStreetNamesWithoutRoadSegment,
                new SuspiciousCase(Category.StreetName, "Straatnamen \"in gebruik\" zonder koppeling met wegverbinding", Severity.Incorrect)
            },
            {
                SuspiciousCasesType.HouseNumbersWithMultipleLinks,
                new SuspiciousCase(Category.Address, "Meervoudig gekoppelde huisnummers", Severity.Suspicious)
            },
            {
                SuspiciousCasesType.SimilarStreetNameNames,
                new SuspiciousCase(Category.StreetName, "Sterk gelijkende straatnamen", Severity.Suspicious)
            },
            {
                SuspiciousCasesType.HouseNumbersNotSituatedBetweenPreviousAndNextHouseNumber,
                new SuspiciousCase(Category.Address, "Huisnummers die niet gelegen zijn tussen het vorige en volgende huisnummer", Severity.Suspicious)
            },
            {
                SuspiciousCasesType.ParcelsLinkedToMultipleHouseNumbers,
                new SuspiciousCase(Category.Parcel, "Percelen gekoppeld aan meerdere huisnummers", Severity.Suspicious)
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
