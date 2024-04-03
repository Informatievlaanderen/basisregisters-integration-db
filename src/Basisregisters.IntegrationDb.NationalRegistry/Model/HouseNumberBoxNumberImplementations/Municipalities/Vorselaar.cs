namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    using System;
    using System.Collections.Generic;

    public class Vorselaar : MunicipalityHouseNumberBoxNumbersBase
    {
        public Vorselaar(string nisCode, string sourceHouseNumber, NationalRegistryIndex index) : base(nisCode, sourceHouseNumber, index)
        { }

        public override bool IsMatch() =>
            NisCode == "13044" &&
            (
                (ContainsOnlyCapitalLetters(IndexSourceValue[0]) && IndexSourceValue[1..] == "000")
                ||
                IndexSourceValue.Trim().Length == 1 && ContainsOnlyCapitalLetters(IndexSourceValue.Trim()[0])
            );

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            if (ContainsOnlyCapitalLetters(IndexSourceValue[0]) && IndexSourceValue[1..] == "000")
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        HouseNumberSourceValue,
                        $"{IndexSourceValue[0]}"
                    )
                };
            }

            if (IndexSourceValue.Trim().Length == 1 && ContainsOnlyCapitalLetters(IndexSourceValue.Trim()[0]))
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
