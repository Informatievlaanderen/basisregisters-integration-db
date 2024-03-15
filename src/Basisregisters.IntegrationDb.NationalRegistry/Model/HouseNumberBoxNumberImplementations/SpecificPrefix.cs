namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations
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

        public override bool IsMatch()
        {
            return Indications.Apartment.Contains(Index.Left, StringComparer.InvariantCultureIgnoreCase)
                   || Indications.FloorNumber.Contains(Index.Left, StringComparer.InvariantCultureIgnoreCase)
                   || Indications.BoxNumber.Contains(Index.Left, StringComparer.InvariantCultureIgnoreCase);
        }

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            // deel 1 aanduiding van appartementnummer: deel 2 wordt busnummer
            if (Indications.Apartment.Contains(Index.Left, StringComparer.InvariantCultureIgnoreCase))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        SourceSourceHouseNumber,
                        Index.Right!.TrimStart('0'))
                };
            }

            // deel 1 aanduiding van busnummer: deel 2 wordt busnummer
            if (Indications.BoxNumber.Contains(Index.Left, StringComparer.InvariantCultureIgnoreCase))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        SourceSourceHouseNumber,
                        Index.Right!.TrimStart('0')),
                    new HouseNumberWithBoxNumber(
                        SourceSourceHouseNumber,
                        Index.SourceValue)
                };
            }

            // deel 1 aanduiding van verdiepnummer: deel1 + deel2 worden busnummer
            if (Indications.FloorNumber.Contains(Index.Left, StringComparer.InvariantCultureIgnoreCase))
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
