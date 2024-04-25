namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    using System;
    using System.Collections.Generic;

    public class Mortsel : MunicipalityHouseNumberBoxNumbersBase
    {
        public Mortsel(string nisCode, string sourceHouseNumber, NationalRegistryIndex index) : base(nisCode, sourceHouseNumber, index)
        { }

        public override bool IsMatch() =>
            NisCode == "11029" &&
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
                        "glv"
                    ),
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
