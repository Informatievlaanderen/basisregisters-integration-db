namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    using System;
    using System.Collections.Generic;

    public class SintPietersLeeuw : MunicipalityHouseNumberBoxNumbersBase
    {
        public SintPietersLeeuw(string nisCode, string sourceHouseNumber, NationalRegistryIndex index) : base(nisCode, sourceHouseNumber, index)
        { }

        public override bool IsMatch() =>
            NisCode == "23077" &&
            (
                (
                    TrimmedIndexSourceValue.Equals("GV", StringComparison.InvariantCultureIgnoreCase)
                    ||
                    IndexSourceValue.Equals("GV-L", StringComparison.InvariantCultureIgnoreCase)
                    ||
                    IndexSourceValue.Equals("GV-R", StringComparison.InvariantCultureIgnoreCase)
                )
                ||
                (
                    TrimmedIndexSourceValue.Length == 4
                    && IsLetter(IndexSourceValue[0])
                    && IndexSourceValue[1] == '-'
                    && IndexSourceValue[2] == 'b'
                    && IsNumeric(IndexSourceValue[3])
                )
                ||
                (
                    IndexSourceValue.StartsWith("bs", StringComparison.InvariantCultureIgnoreCase)
                    && IsNumeric(IndexSourceValue[2..])
                )
            );

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            if (TrimmedIndexSourceValue.Equals("GV", StringComparison.InvariantCultureIgnoreCase)
                ||
                IndexSourceValue.Equals("GV-L", StringComparison.InvariantCultureIgnoreCase)
                ||
                IndexSourceValue.Equals("GV-R", StringComparison.InvariantCultureIgnoreCase))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        HouseNumberSourceValue,
                        IndexSourceValue.Trim()
                    )
                };
            }

            if (TrimmedIndexSourceValue.Length == 4
                && IsLetter(IndexSourceValue[0])
                && IndexSourceValue[1] == '-'
                && IndexSourceValue[2] == 'b'
                && IsNumeric(IndexSourceValue[3]))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        $"{HouseNumberSourceValue}{IndexSourceValue[0]}",
                        IndexSourceValue[3].ToString()
                    )
                };
            }

            if (IndexSourceValue.StartsWith("bs", StringComparison.InvariantCultureIgnoreCase)
                && IsNumeric(IndexSourceValue[2..]))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        HouseNumberSourceValue,
                        int.Parse(IndexSourceValue[2..]).ToString()
                    )
                };
            }

            throw new InvalidOperationException("Invalid use of matches");
        }
    }
}
