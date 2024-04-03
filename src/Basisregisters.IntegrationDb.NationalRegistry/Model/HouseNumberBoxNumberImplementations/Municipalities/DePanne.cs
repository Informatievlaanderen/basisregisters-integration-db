namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    using System;
    using System.Collections.Generic;

    public class DePanne : MunicipalityHouseNumberBoxNumbersBase
    {
        public DePanne(string nisCode, string sourceHouseNumber, NationalRegistryIndex index) : base(nisCode, sourceHouseNumber, index)
        { }

        public override bool IsMatch() =>
            NisCode == "38008" &&
            (
                IsLetter(IndexSourceValue[0])
                ||
                IsNumeric(IndexSourceValue)
            );

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            if (IsLetter(IndexSourceValue[0]))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        HouseNumberSourceValue,
                        IndexSourceValue.Trim()
                    )
                };
            }

            if (IsNumeric(IndexSourceValue))
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
