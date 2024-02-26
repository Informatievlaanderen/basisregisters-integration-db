namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumber
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Slide 31
    /// deel 1 geen aanduiding van bisnummer: aangeleverd huisnummer wordt huisnummer
    /// </summary>
    public sealed class SpecificPrefix : HouseNumberBoxNumbersBase
    {
        public override bool Matches()
        {
            return Index.Left!.Equals("Ap", StringComparison.InvariantCultureIgnoreCase)
                   || Index.Left!.Equals("Vrd", StringComparison.InvariantCultureIgnoreCase)
                   || Index.Left!.Equals("bus", StringComparison.InvariantCultureIgnoreCase);
        }

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            // deel 1 aanduiding van appartementnummer: deel 2 wordt busnummer
            // deel 1 aanduiding van busnummer: deel 2 wordt busnummer
            if (Index.Left!.Equals("Ap", StringComparison.InvariantCultureIgnoreCase)
                || Index.Left!.Equals("bus", StringComparison.InvariantCultureIgnoreCase))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        SourceSourceHouseNumber,
                        Index.Right)
                };
            }

            // deel 1 aanduiding van verdiepnummer: deel1 + deel2 worden busnummer
            if (Index.Left!.Equals("Vrd", StringComparison.InvariantCultureIgnoreCase))
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
