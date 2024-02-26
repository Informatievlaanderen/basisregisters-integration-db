namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumber
{
    using System.Collections.Generic;

    public sealed class NoIndex : HouseNumberBoxNumbersBase
    {
        public NoIndex(string sourceHouseNumber, NationalRegistryIndex index) : base(sourceHouseNumber, index)
        { }

        public override bool IsMatch()
        {
            return string.IsNullOrEmpty(Index) || Index == "0000";
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
    }
}
