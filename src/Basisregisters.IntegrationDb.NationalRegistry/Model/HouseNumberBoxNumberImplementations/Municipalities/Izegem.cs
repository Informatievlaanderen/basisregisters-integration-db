namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    using System;
    using System.Collections.Generic;

    public class Izegem : MunicipalityHouseNumberBoxNumbersBase
    {
        public Izegem(string nisCode, string sourceHouseNumber, NationalRegistryIndex index) : base(nisCode, sourceHouseNumber, index)
        { }

        public override bool IsMatch() => NisCode == "36008" && (
            IndexSourceValue[..3] == "000" && IsNumeric(IndexSourceValue[3])
        );

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            if (IndexSourceValue[..3] == "000" && IsNumeric(IndexSourceValue[3]))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        $"{HouseNumberSourceValue}_{int.Parse(IndexSourceValue)}",
                        null
                    )
                };
            }

            throw new InvalidOperationException("Invalid use of matches");
        }
    }
}
