namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    using System;
    using System.Collections.Generic;

    public class Blankenberge : MunicipalityHouseNumberBoxNumbersBase
    {
        public Blankenberge(string nisCode, string sourceHouseNumber, NationalRegistryIndex index) : base(nisCode, sourceHouseNumber, index)
        { }

        public override bool IsMatch() =>
            NisCode == "31004" &&
            (
                IndexSourceValue.Equals("glvl", StringComparison.InvariantCultureIgnoreCase)
            );

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            if (IndexSourceValue.Equals("glvl", StringComparison.InvariantCultureIgnoreCase))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        HouseNumberSourceValue,
                        "glvl"
                    ),
                    new HouseNumberWithBoxNumber(
                        HouseNumberSourceValue,
                        "GLVL"
                    )
                };
            }

            throw new InvalidOperationException("Invalid use of matches");
        }
    }
}
