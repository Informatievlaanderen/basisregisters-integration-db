namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations
{
    using System.Collections.Generic;

    public sealed class NoIndex : HouseNumberBoxNumbersBase
    {
        public override bool IsMatch()
        {
            return string.IsNullOrEmpty(Index) || Index == "0000" || string.IsNullOrEmpty(Index.Left);
        }

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            return new[]
            {
                new HouseNumberWithBoxNumber(
                    SourceSourceHouseNumber,
                    null)
            };
        }

        public NoIndex(
            string nisCode,
            string sourceHouseNumber,
            NationalRegistryIndex index)
            : base(nisCode, sourceHouseNumber, index)
        { }
    }
}
