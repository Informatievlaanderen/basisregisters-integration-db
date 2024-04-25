namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Brecht : MunicipalityHouseNumberBoxNumbersBase
    {
        public Brecht(string nisCode, string sourceHouseNumber, NationalRegistryIndex index) : base(nisCode, sourceHouseNumber, index)
        { }

        public override bool IsMatch() =>
            NisCode == "11009" &&
            (
                IsLetter(IndexSourceValue[0]) && IndexSourceValue.Contains('/')
                ||
                IndexSourceValue.StartsWith("GV", StringComparison.InvariantCultureIgnoreCase) && IsNumeric(IndexSourceValue[2..])
                ||
                IndexSourceValue[0] == '0' && IsLetter(IndexSourceValue[3])
                ||
                (
                    IndexSourceValue.Equals("GV.L", StringComparison.InvariantCultureIgnoreCase)
                    || IndexSourceValue.Equals("GV.R", StringComparison.InvariantCultureIgnoreCase)
                    || IndexSourceValue.Equals("GLVL", StringComparison.InvariantCultureIgnoreCase)
                )
                ||
                (
                    TrimmedIndexSourceValue.Length == 3
                    && IsNumeric(TrimmedIndexSourceValue[0])
                    && char.ToUpper(TrimmedIndexSourceValue[1]) == 'V'
                    && IsNumeric(TrimmedIndexSourceValue[2])
                )
            );

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            if (IsLetter(IndexSourceValue[0]) && IndexSourceValue.Contains('/'))
            {
                var parts = IndexSourceValue.Split('/');

                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        $"{HouseNumberSourceValue}{parts.First()}",
                        parts.Last()
                    )
                };
            }

            if (IndexSourceValue.StartsWith("GV", StringComparison.InvariantCultureIgnoreCase) && IsNumeric(IndexSourceValue[2..]))
            {
                var boxNumber = IndexSourceValue[2..] == "00" ? IndexSourceValue[..2] : TrimmedIndexSourceValue;

                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        HouseNumberSourceValue,
                        boxNumber
                    )
                };
            }

            if (IndexSourceValue[0] == '0' && IsLetter(IndexSourceValue[3]))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        HouseNumberSourceValue,
                        IndexSourceValue.TrimStart('0')
                    )
                };
            }

            if (IndexSourceValue.Equals("GV.L", StringComparison.InvariantCultureIgnoreCase)
                || IndexSourceValue.Equals("GV.R", StringComparison.InvariantCultureIgnoreCase)
                || IndexSourceValue.Equals("GLVL", StringComparison.InvariantCultureIgnoreCase))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        HouseNumberSourceValue,
                        IndexSourceValue
                    )
                };
            }

            if (TrimmedIndexSourceValue.Length == 3
                && IsNumeric(TrimmedIndexSourceValue[0])
                && char.ToUpper(TrimmedIndexSourceValue[1]) == 'V'
                && IsNumeric(TrimmedIndexSourceValue[2]))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        HouseNumberSourceValue,
                        TrimmedIndexSourceValue
                    )
                };
            }

            throw new InvalidOperationException("Invalid use of matches");
        }
    }
}
