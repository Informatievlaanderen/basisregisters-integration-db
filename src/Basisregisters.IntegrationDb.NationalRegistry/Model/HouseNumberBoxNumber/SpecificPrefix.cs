namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumber
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Slide 31
    /// deel 1 geen aanduiding van bisnummer: aangeleverd huisnummer wordt huisnummer
    /// </summary>
    public sealed class SpecificPrefix : HouseNumberBoxNumbersBase
    {
        private static readonly string[] ApartmentPrefixes =
        {
            "Ap", "App", "Apt"
        };

        private static readonly string[] FloorNumberPrefixes =
        {
            "et", "eta", "éta", "ver", "vdp", "vd", "vr", "vrd", "v", "etg", "ét",
            "et", "ev", "eme", "ème", "ste", "de", "dev", "e", "é"
        };

        private static readonly string[] BoxNumberPrefixes =
        {
            "bus", "bte", "bt", "bu"
        };

        public override bool Matches()
        {
            return ApartmentPrefixes.Contains(Index.Left, StringComparer.InvariantCultureIgnoreCase)
                   || FloorNumberPrefixes.Contains(Index.Left, StringComparer.InvariantCultureIgnoreCase)
                   || BoxNumberPrefixes.Contains(Index.Left, StringComparer.InvariantCultureIgnoreCase);
        }

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            // deel 1 aanduiding van appartementnummer: deel 2 wordt busnummer
            // deel 1 aanduiding van busnummer: deel 2 wordt busnummer
            if (ApartmentPrefixes.Contains(Index.Left, StringComparer.InvariantCultureIgnoreCase)
                || BoxNumberPrefixes.Contains(Index.Left, StringComparer.InvariantCultureIgnoreCase))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        SourceSourceHouseNumber,
                        Index.Right!.TrimStart('0'))
                };
            }

            // deel 1 aanduiding van verdiepnummer: deel1 + deel2 worden busnummer
            if (FloorNumberPrefixes.Contains(Index.Left, StringComparer.InvariantCultureIgnoreCase))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        SourceSourceHouseNumber,
                        $"{Index.Left}{Index.Right}")
                };
            }

            throw new InvalidOperationException("Invalid use of matches");
        }

        public SpecificPrefix(string sourceHouseNumber, NationalRegistryIndex index)
            : base(sourceHouseNumber, index)
        { }
    }
}
