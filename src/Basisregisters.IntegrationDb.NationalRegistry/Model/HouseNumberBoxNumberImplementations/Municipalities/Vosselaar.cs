namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    using System;
    using System.Collections.Generic;

    public class Vosselaar : MunicipalityHouseNumberBoxNumbersBase
    {
        public Vosselaar(string nisCode, string sourceHouseNumber, NationalRegistryIndex index) : base(nisCode, sourceHouseNumber, index)
        { }

        public override bool IsMatch() => NisCode == "13046" && (
            IndexSourceValue.StartsWith("b", StringComparison.InvariantCultureIgnoreCase) && IsNumeric(IndexSourceValue[1..])
        );

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            if (IndexSourceValue.StartsWith("b", StringComparison.InvariantCultureIgnoreCase) && IsNumeric(IndexSourceValue[1..]))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        HouseNumberSourceValue,
                        int.Parse(IndexSourceValue[1..]).ToString()
                    )
                };
            }

            throw new InvalidOperationException("Invalid use of matches");
        }
    }
}
