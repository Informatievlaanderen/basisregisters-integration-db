namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    public class Maasmechelen : MunicipalityHouseNumberBoxNumbersBase
    {
        public Maasmechelen(string nisCode, string sourceHouseNumber, NationalRegistryIndex index) : base(nisCode, sourceHouseNumber, index)
        { }

        public override bool IsMatch() => NisCode == "73107" && (
            (IndexSourceValue[0] == '/' && IsLetter(IndexSourceValue[1]) && IndexSourceValue[2] == '-' && IsNumeric(IndexSourceValue[3]))
            ||
            (IndexSourceValue[0] == '/' && IsLetter(IndexSourceValue[1]) && IsNumeric(IndexSourceValue[2..]))
            ||
            (IsLetter(IndexSourceValue[0]) && IsNumeric(IndexSourceValue[1..3]) && int.Parse(IndexSourceValue[1..3]) == 0 && IndexSourceValue[3] == ' ')
        );

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            if (IndexSourceValue[0] == '/' && IsLetter(IndexSourceValue[1]) && IndexSourceValue[2] == '-' && IsNumeric(IndexSourceValue[3]))
            {
                var boxNumber = int.Parse(IndexSourceValue[3..]);

                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        $"{HouseNumberSourceValue}{IndexSourceValue[1]}",
                        boxNumber > 0 ? boxNumber.ToString() : null
                    )
                };
            }
            if (IndexSourceValue[0] == '/' && IsLetter(IndexSourceValue[1]) && IsNumeric(IndexSourceValue[2..]))
            {
                var boxNumber = int.Parse(IndexSourceValue[2..]);

                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        $"{HouseNumberSourceValue}{IndexSourceValue[1]}",
                        boxNumber > 0 ? boxNumber.ToString() : null
                    )
                };
            }


            if (IsLetter(IndexSourceValue[0]) && IsNumeric(IndexSourceValue[1..3]) && int.Parse(IndexSourceValue[1..3]) == 0 && IndexSourceValue[3] == ' ')
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        $"{HouseNumberSourceValue}{IndexSourceValue[0]}",
                        null
                    )
                };
            }

            throw new InvalidOperationException("Invalid use of matches");
        }
    }
}
