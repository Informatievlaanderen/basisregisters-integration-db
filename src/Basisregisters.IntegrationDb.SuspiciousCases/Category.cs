namespace Basisregisters.IntegrationDb.SuspiciousCases
{
    using System;

    public enum Category
    {
        Address = 1,
        Parcel = 2,
        RoadSegment = 3,
        Building = 4,
        BuildingUnit = 5,
        StreetName = 6
    }

    public static class CategoryExtensions
    {
        public static string ToDutchString(this Category category)
        {
            return category switch
            {
                Category.Address => "Adres",
                Category.Parcel => "Perceel",
                Category.RoadSegment => "Wegsegment",
                Category.Building => "Gebouw",
                Category.BuildingUnit => "Gebouweenheid",
                Category.StreetName => "Straatnaam",
                _ => throw new ArgumentOutOfRangeException(nameof(category), category, null)
            };
        }
    }
}
