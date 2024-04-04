namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    using System.Collections.Generic;

    public class Lint : MunicipalityHouseNumberBoxNumbersBase
    {
        public Lint(string nisCode, string sourceHouseNumber, NationalRegistryIndex index) : base(nisCode, sourceHouseNumber, index)
        { }

        public override bool IsMatch() => NisCode == "11025";

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            return new[]
            {
                new HouseNumberWithBoxNumber(
                    HouseNumberSourceValue,
                    IndexSourceValue.Trim()
                )
            };
        }
    }
}
