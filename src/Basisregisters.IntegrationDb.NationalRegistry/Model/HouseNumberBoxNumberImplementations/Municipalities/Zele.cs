namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    using System;
    using System.Collections.Generic;

    public class Zele : MunicipalityHouseNumberBoxNumbersBase
    {
        public Zele(string nisCode, string sourceHouseNumber, NationalRegistryIndex index) : base(nisCode, sourceHouseNumber, index)
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
                    IndexSourceValue.StartsWith("AW", StringComparison.InvariantCultureIgnoreCase)
                    ||
                    IndexSourceValue.StartsWith("BW", StringComparison.InvariantCultureIgnoreCase)
                )
            );

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            if (IndexSourceValue.StartsWith("b", StringComparison.InvariantCultureIgnoreCase) && IsNumeric(IndexSourceValue[1..]))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        SourceSourceHouseNumber,
                        IndexSourceValue.Trim()
                    )
                };
            }

            if (IndexSourceValue.Contains('.'))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        SourceSourceHouseNumber,
                        IndexSourceValue.Trim()
                    )
                };
            }

            if (IndexSourceValue.StartsWith("W", StringComparison.InvariantCultureIgnoreCase) && IndexSourceValue[1..3] == "00")
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        SourceSourceHouseNumber,
                        IndexSourceValue.Replace("00", string.Empty).Trim()
                    )
                };
            }

            if (IndexSourceValue.StartsWith("W", StringComparison.InvariantCultureIgnoreCase) && IndexSourceValue[1] == '0')
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        SourceSourceHouseNumber,
                        IndexSourceValue.Trim()
                    )
                };
            }

            if (IndexSourceValue.StartsWith("AW", StringComparison.InvariantCultureIgnoreCase))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        $"{SourceSourceHouseNumber}A",
                        $"W{int.Parse(IndexSourceValue[2..])}"
                    )
                };
            }

            if (IndexSourceValue.StartsWith("BW", StringComparison.InvariantCultureIgnoreCase))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        $"{SourceSourceHouseNumber}B",
                        $"W{int.Parse(IndexSourceValue[2..])}"
                    )
                };
            }

            throw new InvalidOperationException("Invalid use of matches");
        }
    }
}
