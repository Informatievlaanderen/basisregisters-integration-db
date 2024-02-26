namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumber
{
    using System.Collections.Generic;

    /// <summary>
    /// Slide 30
    /// aangeleverde index waarvan
    ///     deel 1 niet-numeriek
    ///     en deel 2 numeriek en groter dan nul (0)
    /// -> deel 1 aanduiding van bisnummer: aangeleverd huisnummer + deel 1 wordt huisnummer; deel 2 wordt busnummer
    /// </summary>
    public sealed class NonNumericFollowedByNumberGreaterThanZero : HouseNumberBoxNumbersBase
    {
        public override bool IsMatch()
        {
            return ContainsOnlyLetters(Index.Left!) && IsGreaterThanZero(Index.Right!);
        }

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            return new[]
            {
                new HouseNumberWithBoxNumber(
                    $"{SourceSourceHouseNumber}{Index.Left}",
                    Index.Right!.TrimStart('0'))
            };
        }

        public NonNumericFollowedByNumberGreaterThanZero(string sourceHouseNumber, NationalRegistryIndex index)
            : base(sourceHouseNumber, index)
        { }
    }
}
