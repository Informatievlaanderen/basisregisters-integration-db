namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    using System;
    using System.Collections.Generic;

    public class Machelen : MunicipalityHouseNumberBoxNumbersBase
    {
        public Machelen(string nisCode, string sourceHouseNumber, NationalRegistryIndex index) : base(nisCode, sourceHouseNumber, index)
        { }

        public override bool IsMatch() =>
            NisCode == "23047" &&
            (
                IndexSourceValue.StartsWith("BS")
            );

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            if (IndexSourceValue.StartsWith("BS"))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        HouseNumberSourceValue,
                        IndexSourceValue.Trim()
                    )
                };
            }

            throw new InvalidOperationException("Invalid use of matches");
        }
    }
}
