namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    using System;
    using System.Collections.Generic;

    public class Halen : MunicipalityHouseNumberBoxNumbersBase
    {
        public Halen(string nisCode, string sourceHouseNumber, NationalRegistryIndex index) : base(nisCode, sourceHouseNumber, index)
        { }

        public override bool IsMatch() =>
            NisCode == "71020" &&
            (
                IndexSourceValue.StartsWith("b", StringComparison.InvariantCultureIgnoreCase) && IsNumeric(IndexSourceValue[1..])
                ||
                IndexSourceValue.Contains('.')
                ||
                IndexSourceValue.StartsWith("W", StringComparison.InvariantCultureIgnoreCase) && IndexSourceValue[1..3] == "00"
                ||
                IndexSourceValue.StartsWith("W", StringComparison.InvariantCultureIgnoreCase) && IndexSourceValue[1] == '0'
                ||
                (
                    (IndexSourceValue.StartsWith("AW", StringComparison.InvariantCultureIgnoreCase)
                    ||
                    IndexSourceValue.StartsWith("BW", StringComparison.InvariantCultureIgnoreCase))
                    && IsNumeric(IndexSourceValue[2..])
                )
                ||
                (IsLetter(IndexSourceValue[0]) && IndexSourceValue[1..3] == "000")
                ||
                (IsLetter(IndexSourceValue[0]) && IndexSourceValue[1] == '/')
            );

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            if (IndexSourceValue.StartsWith("b", StringComparison.InvariantCultureIgnoreCase) && IsNumeric(IndexSourceValue[1..]))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        HouseNumberSourceValue,
                        IndexSourceValue.Trim()
                    )
                };
            }

            if (IndexSourceValue.Contains('.'))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        HouseNumberSourceValue,
                        IndexSourceValue.Trim()
                    )
                };
            }

            if (IndexSourceValue.StartsWith("W", StringComparison.InvariantCultureIgnoreCase) && IndexSourceValue[1..3] == "00")
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        HouseNumberSourceValue,
                        IndexSourceValue.Replace("00", string.Empty).Trim()
                    )
                };
            }

            if (IndexSourceValue.StartsWith("W", StringComparison.InvariantCultureIgnoreCase) && IndexSourceValue[1] == '0')
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        HouseNumberSourceValue,
                        IndexSourceValue.Trim()
                    )
                };
            }

            if (IndexSourceValue.StartsWith("AW", StringComparison.InvariantCultureIgnoreCase) && IsNumeric(IndexSourceValue[2..]))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        $"{HouseNumberSourceValue}A",
                        $"W{int.Parse(IndexSourceValue[2..])}"
                    )
                };
            }

            if (IndexSourceValue.StartsWith("BW", StringComparison.InvariantCultureIgnoreCase) && IsNumeric(IndexSourceValue[2..]))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        $"{HouseNumberSourceValue}B",
                        $"W{int.Parse(IndexSourceValue[2..])}"
                    )
                };
            }

            if (IsLetter(IndexSourceValue[0]) && IndexSourceValue[1..3] == "000")
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        $"{HouseNumberSourceValue}{IndexSourceValue[0]}",
                        null
                    )
                };
            }

            if (IsLetter(IndexSourceValue[0]) && IndexSourceValue[1] == '/')
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        $"{HouseNumberSourceValue}{IndexSourceValue[0]}",
                        IndexSourceValue[2..].Trim()
                    )
                };
            }

            throw new InvalidOperationException("Invalid use of matches");
        }
    }
}
