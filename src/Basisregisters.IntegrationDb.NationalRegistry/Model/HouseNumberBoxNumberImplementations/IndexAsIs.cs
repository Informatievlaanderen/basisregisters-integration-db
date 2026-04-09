namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations
{
    using System.Collections.Generic;

    /// <summary>
    /// Nieuw: index as is overnemen
    /// </summary>
    public sealed class IndexAsIs : HouseNumberBoxNumbersBase
    {
        public override bool IsMatch()
        {
            return true;
        }

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            return new[]
            {
                new HouseNumberWithBoxNumber(
                    HouseNumberSourceValue,
                    Index.SourceValue!.Trim())
            };
        }

        public IndexAsIs(
            string nisCode,
            string sourceHouseNumber,
            NationalRegistryIndex index)
            : base(nisCode, sourceHouseNumber, index)
        { }
    }
}
