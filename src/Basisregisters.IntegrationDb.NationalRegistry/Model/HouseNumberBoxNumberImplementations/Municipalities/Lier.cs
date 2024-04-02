namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    using System;
    using System.Collections.Generic;

    public class Lier : MunicipalityHouseNumberBoxNumbersBase
    {
        public Lier(string nisCode, string sourceHouseNumber, NationalRegistryIndex index) : base(nisCode, sourceHouseNumber, index)
        { }

        public override bool IsMatch() =>
            NisCode == "12021" &&
            (
                (IndexSourceValue!.StartsWith('B') && IsNumeric(IndexSourceValue[1..].Trim()))
                ||
                (ContainsOnlyCapitalLetters(IndexSourceValue[0]) && IndexSourceValue[1] == 'B' && IsNumeric(IndexSourceValue[2..]))
            );

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            if (IndexSourceValue!.StartsWith('B') && IsNumeric(IndexSourceValue[1..]))
            {
                return new List<HouseNumberWithBoxNumber>
                {
                    new HouseNumberWithBoxNumber(
                        SourceSourceHouseNumber,
                        int.Parse(IndexSourceValue[1..].Trim()).ToString())
                };
            }

            if (ContainsOnlyCapitalLetters(IndexSourceValue[0]) && IndexSourceValue[1] == 'B' && IsNumeric(IndexSourceValue[2..]))
            {
                return new List<HouseNumberWithBoxNumber>
                {
                    new HouseNumberWithBoxNumber(
                        $"{SourceSourceHouseNumber}{IndexSourceValue[0]}",
                        int.Parse(IndexSourceValue[2..].Trim()).ToString())
                };
            }

            throw new InvalidOperationException("Invalid use of matches");
        }
    }
}
