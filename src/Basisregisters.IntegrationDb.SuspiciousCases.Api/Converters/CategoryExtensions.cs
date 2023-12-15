﻿namespace Basisregisters.IntegrationDb.SuspiciousCases.Api.Converters
{
    using System;
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
                Category.Buildingunit => Categorie.Gebouweenheid,
                Category.StreetName => Categorie.Straatnaam,
                _ => throw new ArgumentOutOfRangeException(nameof(category), category, null)
            };
        }
    }
}
