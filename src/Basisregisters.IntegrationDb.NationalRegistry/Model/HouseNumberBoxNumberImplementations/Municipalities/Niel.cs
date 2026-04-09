namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    using System;
    using System.Collections.Generic;

    public sealed class Niel : MunicipalityHouseNumberBoxNumbersBase
    {
        public Niel(string nisCode, string sourceHouseNumber, NationalRegistryIndex index) : base(nisCode, sourceHouseNumber, index)
        { }

        public override bool IsMatch() => NisCode == "11030" && (ContainsOnlyCapitalLetters(IndexSourceValue[0]) && IsNumeric(IndexSourceValue[1..])
            || (IndexSourceValue.Length == 4 && IsLetter(IndexSourceValue[0]) && IndexSourceValue[1] == '/' && IndexSourceValue[2] == 'B'));

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            if (ContainsOnlyCapitalLetters(IndexSourceValue[0]) && IsNumeric(IndexSourceValue[1..]))
            {
                return
                [
                    new HouseNumberWithBoxNumber(
                        HouseNumberSourceValue,
                        Index.Right!.TrimStart('0')
                    )
                ];
            }

            if (IndexSourceValue.Length == 4 && IsLetter(IndexSourceValue[0]) && IndexSourceValue[1] == '/' && IndexSourceValue[2] == 'B')
            {
                return
                [
                    new HouseNumberWithBoxNumber(
                        HouseNumberSourceValue + IndexSourceValue[0],
                        IndexSourceValue[3].ToString()
                    )
                ];
            }

            throw new InvalidOperationException("Invalid use of matches");
        }
    }
}
