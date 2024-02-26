namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumber
{
    using System.Collections.Generic;

    /// <summary>
    /// Slide 33
    /// aangeleverde index waarvan deel 1 numeriek, deel 3 numeriek en zonder deel 4
    /// -> aangeleverd huisnummer wordt huisnummer, aangeleverde index wordt busnummer
    /// </summary>
    public sealed class SeparatorBetweenNumbers : HouseNumberBoxNumbersBase
    {
        public override bool IsMatch()
        {
            return int.TryParse(Index.Left, out _)
                   && IsNumeric(Index.RightPartOne) && string.IsNullOrEmpty(Index.RightPartTwo);
        }

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            return new[]
            {
                new HouseNumberWithBoxNumber(
                    SourceSourceHouseNumber,
                    Index.SourceValue)
            };
        }

        public SeparatorBetweenNumbers(string sourceHouseNumber, NationalRegistryIndex index)
            : base(sourceHouseNumber, index)
        { }
    }
}
