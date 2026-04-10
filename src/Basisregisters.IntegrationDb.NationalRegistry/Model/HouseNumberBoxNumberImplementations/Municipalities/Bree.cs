namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    using System;
    using System.Collections.Generic;

    public sealed class Bree : MunicipalityHouseNumberBoxNumbersBase
    {
        public Bree(string nisCode, string sourceHouseNumber, NationalRegistryIndex index) : base(nisCode, sourceHouseNumber, index)
        { }

        public override bool IsMatch() => NisCode == "72004" && (
            (IndexSourceValue.Length == 4 && ContainsOnlyCapitalLetters(IndexSourceValue[0]) && IndexSourceValue[2] == '/')
            ||
            (TrimmedIndexSourceValue.Length == 3 && TrimmedIndexSourceValue[1] == '/'));

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            if (IndexSourceValue.Length == 4 && ContainsOnlyCapitalLetters(IndexSourceValue[0]) && IndexSourceValue[2] == '/')
            {
                return
                [
                    new HouseNumberWithBoxNumber(
                        HouseNumberSourceValue + IndexSourceValue[0],
                        IndexSourceValue[1..]
                    )
                ];
            }

            if((TrimmedIndexSourceValue.Length == 3 && TrimmedIndexSourceValue[1] == '/'))
            {
                return
                [
                    new HouseNumberWithBoxNumber(
                        HouseNumberSourceValue,
                        TrimmedIndexSourceValue
                    ),
                    new HouseNumberWithBoxNumber(
                        HouseNumberSourceValue,
                        TrimmedIndexSourceValue[2].ToString()
                    )
                ];
            }

            throw new InvalidOperationException("Invalid use of matches");
        }
    }
}
