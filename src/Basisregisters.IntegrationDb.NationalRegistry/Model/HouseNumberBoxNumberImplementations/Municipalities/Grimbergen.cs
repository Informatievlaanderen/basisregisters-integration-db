namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    using System.Collections.Generic;

    public class Grimbergen : MunicipalityHouseNumberBoxNumbersBase
    {
        public Grimbergen(string nisCode, string sourceHouseNumber, NationalRegistryIndex index) : base(nisCode, sourceHouseNumber, index)
        { }

        public override bool IsMatch() => NisCode == "23025";

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
