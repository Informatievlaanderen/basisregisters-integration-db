namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    using System;
    using System.Collections.Generic;

    public class Borsbeek : MunicipalityHouseNumberBoxNumbersBase
    {
        public Borsbeek(string nisCode, string sourceHouseNumber, NationalRegistryIndex index) : base(nisCode, sourceHouseNumber, index)
        { }

        public override bool IsMatch() => NisCode == "11007";

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            if (IndexSourceValue.Equals("gel0", StringComparison.InvariantCultureIgnoreCase))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        HouseNumberSourceValue,
                        "0.0"
                    )
                };
            }

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
