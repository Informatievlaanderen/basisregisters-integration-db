namespace Basisregisters.IntegrationDb.Api.Abstractions.SuspiciousCase.Converters
{
    using System;
    using Basisregisters.IntegrationDb.SuspiciousCases;
    using List;

    public static class CategoryExtensions
    {
        public static Categorie Map(this Category category)
        {
            return category switch
            {
                Category.Address => Categorie.Adres,
                Category.Parcel => Categorie.Perceel,
                Category.RoadSegment => Categorie.Wegsegment,
                Category.Building => Categorie.Gebouw,
                Category.BuildingUnit => Categorie.Gebouweenheid,
                Category.StreetName => Categorie.Straatnaam,
                _ => throw new ArgumentOutOfRangeException(nameof(category), category, null)
            };
        }
    }
}
