namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations
{
    using System.Collections.Generic;

    /// <summary>
    /// Slide 35
    /// Deel 1 is numeriek en deel 3 aanduiding van appartement
    /// -> aangeleverd huisnummer + ‘_’ + deel 1 wordt huisnummer, deel 3 wordt busnummer
    /// </summary>
    public sealed class NumericFollowedByNonNumeric : HouseNumberBoxNumbersBase
    {
        public override bool IsMatch()
        {
            return int.TryParse(Index.Left, out _)
                   && !string.IsNullOrEmpty(Index.RightPartOne) && !IsNumeric(Index.RightPartOne)
                   && string.IsNullOrEmpty(Index.RightPartTwo);
        }

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            return new[]
            {
                new HouseNumberWithBoxNumber(
                    $"{HouseNumberSourceValue}_{int.Parse(Index.Left!)}",
                    Index.RightPartOne)
            };
        }

        public NumericFollowedByNonNumeric(
            string nisCode,
            string sourceHouseNumber,
            NationalRegistryIndex index)
            : base(nisCode, sourceHouseNumber, index)
        { }
    }
}
