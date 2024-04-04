namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    using System.Collections.Generic;

    public class Wommelgem : MunicipalityHouseNumberBoxNumbersBase
    {
        public Wommelgem(string nisCode, string sourceHouseNumber, NationalRegistryIndex index) : base(nisCode, sourceHouseNumber, index)
        { }

        public override bool IsMatch() => NisCode == "11052";

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
