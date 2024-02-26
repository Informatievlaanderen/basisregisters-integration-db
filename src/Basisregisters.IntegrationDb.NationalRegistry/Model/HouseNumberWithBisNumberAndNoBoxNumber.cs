namespace Basisregisters.IntegrationDb.NationalRegistry.Model
{
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
                    $"{PreCalculatedHouseNumber}{Index!.Left}",
                    null)
            };
        }

        public HouseNumberWithBisNumberAndNoBoxNumber(string houseNumber, NationalRegistryIndex index)
            : base(houseNumber, index)
        { }
    }

    /// <summary>
    /// aangeleverde index waarvan
    ///     deel 1 niet-numeriek
    ///     en deel 2 numeriek en groter dan nul (0)
    /// -> deel 1 aanduiding van bisnummer: aangeleverd huisnummer + deel 1 wordt huisnummer; deel 2 wordt busnummer
    /// </summary>
    public sealed class NonNumericHouseNumberWithBisNumberAndNumericBoxNumber : HouseNumberBoxNumbersBase
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
                    $"{PreCalculatedHouseNumber}{Index!.Left}",
                    Index.Right!.TrimStart('0'))
            };
        }

        public NonNumericHouseNumberWithBisNumberAndNumericBoxNumber(string houseNumber, NationalRegistryIndex? index)
            : base(houseNumber, index)
        { }
    }
}
