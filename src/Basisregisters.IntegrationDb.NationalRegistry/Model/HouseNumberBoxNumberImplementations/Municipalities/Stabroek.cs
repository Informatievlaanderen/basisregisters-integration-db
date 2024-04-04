namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    using System;
    using System.Collections.Generic;

    public class Stabroek : MunicipalityHouseNumberBoxNumbersBase
    {
        public Stabroek(string nisCode, string sourceHouseNumber, NationalRegistryIndex index) : base(nisCode, sourceHouseNumber, index)
        { }

        public override bool IsMatch() =>
            NisCode == "11044" &&
            (
                IndexSourceValue.Equals("GLVL", StringComparison.InvariantCultureIgnoreCase)
                ||
                IndexSourceValue[1..3].Equals("VD", StringComparison.InvariantCultureIgnoreCase)
                ||
                IndexSourceValue.StartsWith("GL", StringComparison.InvariantCultureIgnoreCase)
            );

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            if (IndexSourceValue.Equals("GLVL", StringComparison.InvariantCultureIgnoreCase)
                || IndexSourceValue[1..3].Equals("VD", StringComparison.InvariantCultureIgnoreCase)
                || IndexSourceValue.StartsWith("GL", StringComparison.InvariantCultureIgnoreCase))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        HouseNumberSourceValue,
                        IndexSourceValue
                    )
                };
            }

            throw new InvalidOperationException("Invalid use of matches");
        }
    }
}
