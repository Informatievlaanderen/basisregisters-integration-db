namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    using System;
    using System.Collections.Generic;

    public sealed class ErpeMere : MunicipalityHouseNumberBoxNumbersBase
    {
        public ErpeMere(string nisCode, string sourceHouseNumber, NationalRegistryIndex index) : base(nisCode, sourceHouseNumber, index)
        { }

        public override bool IsMatch() => NisCode == "41082" && IndexSourceValue.Length == 4 && (
            (IndexSourceValue.Contains("bs") && IsNumeric(IndexSourceValue[3]))
            ||
            (ContainsOnlyCapitalLetters(IndexSourceValue[0]) && IndexSourceValue[1] == 'b' && IsNumeric(IndexSourceValue[2..])));

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            if (ContainsOnlyCapitalLetters(IndexSourceValue[0]) && IndexSourceValue.Contains("bs") && IsNumeric(IndexSourceValue[3]))
            {
                return
                [
                    new HouseNumberWithBoxNumber(
                        HouseNumberSourceValue + IndexSourceValue[0],
                        IndexSourceValue[3].ToString()
                    )
                ];
            }

            if (IndexSourceValue.StartsWith("bs") && IsNumeric(IndexSourceValue[3]))
            {
                return
                [
                    new HouseNumberWithBoxNumber(
                        HouseNumberSourceValue,
                        IndexSourceValue[2..].TrimStart('0')
                    )
                ];
            }

            if (ContainsOnlyCapitalLetters(IndexSourceValue[0]) && IndexSourceValue[1] == 'b' && IsNumeric(IndexSourceValue[2..]))
            {
                return
                [
                    new HouseNumberWithBoxNumber(
                        $"{HouseNumberSourceValue}{IndexSourceValue[0]}",
                        IndexSourceValue[2..].TrimStart('0')
                    )
                ];
            }


            throw new InvalidOperationException("Invalid use of matches");
        }
    }
}
