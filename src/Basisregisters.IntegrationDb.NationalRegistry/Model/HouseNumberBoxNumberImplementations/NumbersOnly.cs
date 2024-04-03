namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations
{
    using System.Collections.Generic;

    /// <summary>
    /// Slide 32
    /// aangeleverde index waarvan deel 1 numeriek en zonder deel 2 en zonder deel 3
    /// -> aangeleverd huisnummer wordt huisnummer, deel 1 wordt busnummer
    /// </summary>
    public sealed class NumbersOnly : HouseNumberBoxNumbersBase
    {
        public override bool IsMatch()
        {
            return int.TryParse(Index.Left!, out _) && string.IsNullOrEmpty(Index.Right);
        }

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            return new[]
            {
                new HouseNumberWithBoxNumber(
                    SourceSourceHouseNumber,
                    $"{int.Parse(Index.Left!)}")
            };
        }

        public NumbersOnly(
            string nisCode,
            string sourceHouseNumber,
            NationalRegistryIndex index)
            : base(nisCode, sourceHouseNumber, index)
        { }
    }
}
