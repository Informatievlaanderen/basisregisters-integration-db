namespace Basisregisters.IntegrationDb.NationalRegistry.Model
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Slide 29
    /// aangeleverde index waarvan
    ///     deel 1 niet-numeriek
    ///     en deel 2 numeriek en gelijk aan nul (0)
    /// -> deel 1 aanduiding van bisnummer: aangeleverd huisnummer + deel 1 wordt huisnummer; geen busnummer
    /// </summary>
    public sealed class HouseNumberWithBisNumberAndNoBoxNumber : HouseNumberBoxNumbersBase
    {
        public override bool Matches()
        {
            return ContainsOnlyLetters(Index!.Left!) && ContainsOnlyZeroes(Index.Right!);
        }

        public override IEnumerable<HouseNumberWithBoxNumber> GetValues()
        {
            return new[]
            {
                new HouseNumberWithBoxNumber(
                    $"{SourceSourceHouseNumber}{Index!.Left}",
                    null)
            };
        }

        public HouseNumberWithBisNumberAndNoBoxNumber(string sourceHouseNumber, NationalRegistryIndex index)
            : base(sourceHouseNumber, index)
        { }
    }

    /// <summary>
    /// Slide 30
    /// aangeleverde index waarvan
    ///     deel 1 niet-numeriek
    ///     en deel 2 numeriek en groter dan nul (0)
    /// -> deel 1 aanduiding van bisnummer: aangeleverd huisnummer + deel 1 wordt huisnummer; deel 2 wordt busnummer
    /// </summary>
    public sealed class HouseNumberWithBisNumberAndNumericBoxNumber : HouseNumberBoxNumbersBase
    {
        public override bool Matches()
        {
            return ContainsOnlyLetters(Index!.Left!) && IsGreaterThanZero(Index.Right!);
        }

        public override IEnumerable<HouseNumberWithBoxNumber> GetValues()
        {
            return new[]
            {
                new HouseNumberWithBoxNumber(
                    $"{SourceSourceHouseNumber}{Index!.Left}",
                    Index.Right!.TrimStart('0'))
            };
        }

        public HouseNumberWithBisNumberAndNumericBoxNumber(string sourceHouseNumber, NationalRegistryIndex? index)
            : base(sourceHouseNumber, index)
        { }
    }

    /// <summary>
    /// Slide 31
    /// deel 1 geen aanduiding van bisnummer: aangeleverd huisnummer wordt huisnummer
    /// </summary>
    public sealed class HouseNumberWithNoBisNumberAndBoxNumber : HouseNumberBoxNumbersBase
    {
        public override bool Matches()
        {
            return Index!.Left!.Equals("Ap", StringComparison.InvariantCultureIgnoreCase)
                   || Index.Left!.Equals("Vrd", StringComparison.InvariantCultureIgnoreCase)
                   || Index.Left!.Equals("bus", StringComparison.InvariantCultureIgnoreCase);
        }

        public override IEnumerable<HouseNumberWithBoxNumber> GetValues()
        {
            if (Index!.Left!.Equals("Ap", StringComparison.InvariantCultureIgnoreCase)
                || Index.Left!.Equals("bus", StringComparison.InvariantCultureIgnoreCase))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        SourceSourceHouseNumber,
                        Index!.Right)
                };
            }

            return new[]
            {
                new HouseNumberWithBoxNumber(
                    SourceSourceHouseNumber,
                    $"{Index!.Left}{Index.Right}")
            };
        }

        public HouseNumberWithNoBisNumberAndBoxNumber(string sourceHouseNumber, NationalRegistryIndex index)
            : base(sourceHouseNumber, index)
        { }
    }
}
