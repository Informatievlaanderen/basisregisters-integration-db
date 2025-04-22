namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    using System;
    using System.Collections.Generic;

    public class LokerenMoerbeke : MunicipalityHouseNumberBoxNumbersBase
    {
        public LokerenMoerbeke(string nisCode, string sourceHouseNumber, NationalRegistryIndex index) : base(nisCode, sourceHouseNumber, index)
        { }

        public override bool IsMatch() =>
            NisCode == "46029" &&
            (
                ContainsOnlyCapitalLetters(IndexSourceValue[0]) && IsNumeric(IndexSourceValue[1..])
            );

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            if (ContainsOnlyCapitalLetters(IndexSourceValue[0]) && IsNumeric(IndexSourceValue[1..]))
            {
                return
                [
                    new HouseNumberWithBoxNumber(
                        HouseNumberSourceValue,
                        IndexSourceValue
                    )
                ];
            }

            throw new InvalidOperationException("Invalid use of matches");
        }
    }
}
