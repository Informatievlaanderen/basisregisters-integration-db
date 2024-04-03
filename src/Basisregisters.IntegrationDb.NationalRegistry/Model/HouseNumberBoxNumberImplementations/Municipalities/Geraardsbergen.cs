namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Geraardsbergen : MunicipalityHouseNumberBoxNumbersBase
    {
        public Geraardsbergen(string nisCode, string sourceHouseNumber, NationalRegistryIndex index) : base(nisCode, sourceHouseNumber, index)
        { }

        public override bool IsMatch() =>
            NisCode == "41018" &&
            (
                (IsLetter(IndexSourceValue[0]) && IndexSourceValue[1..] == "000")
                ||
                (char.ToUpper(IndexSourceValue[0]) == 'B' && IsNumberGreaterThanZero(IndexSourceValue[1..]))
                ||
                (IsLetter(IndexSourceValue[0]) && IndexSourceValue.Contains('/') && IsNumeric(IndexSourceValue[3]))
                ||
                (IsLetter(IndexSourceValue[0]) && char.ToUpper(IndexSourceValue[1]) == 'B' && IsNumeric(IndexSourceValue[2..]))
            );

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            if (IsLetter(IndexSourceValue[0]) && IndexSourceValue[1..] == "000")
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        $"{SourceSourceHouseNumber}{IndexSourceValue[0]}",
                        null
                    )
                };
            }

            if (char.ToUpper(IndexSourceValue[0]) == 'B' && IsNumberGreaterThanZero(IndexSourceValue[1..]))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        SourceSourceHouseNumber,
                        int.Parse(IndexSourceValue[1..]).ToString()
                    )
                };
            }

            if (IsLetter(IndexSourceValue[0]) && IndexSourceValue.Contains('/') && IsNumeric(IndexSourceValue[3]))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        $"{SourceSourceHouseNumber}{IndexSourceValue[0]}",
                        int.Parse(string.Join(string.Empty, IndexSourceValue.Split('/').Last().Where(char.IsDigit))).ToString()
                    )
                };
            }

            if (IsLetter(IndexSourceValue[0]) && char.ToUpper(IndexSourceValue[1]) == 'B' && IsNumeric(IndexSourceValue[2..]))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        $"{SourceSourceHouseNumber}{IndexSourceValue[0]}",
                        int.Parse(IndexSourceValue[2..]).ToString()
                    )
                };
            }

            throw new InvalidOperationException("Invalid use of matches");
        }
    }
}
