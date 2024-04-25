namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    using System;
    using System.Collections.Generic;

    public class Koksijde : MunicipalityHouseNumberBoxNumbersBase
    {
        public Koksijde(string nisCode, string sourceHouseNumber, NationalRegistryIndex index) : base(nisCode, sourceHouseNumber, index)
        { }

        public override bool IsMatch() =>
            NisCode == "38014" &&
            (
                (
                    IndexSourceValue.StartsWith("KV", StringComparison.InvariantCultureIgnoreCase)
                    && IsNumeric(IndexSourceValue[2..])
                )
                ||
                (
                    IndexSourceValue.StartsWith("GV", StringComparison.InvariantCultureIgnoreCase)
                    && IsNumeric(IndexSourceValue[2..])
                )
                ||
                (IndexSourceValue.StartsWith("XGV") && IsNumeric(IndexSourceValue[3]))
                ||
                (
                    IndexSourceValue.StartsWith("K", StringComparison.InvariantCultureIgnoreCase)
                    && IndexSourceValue.Trim().Length == 4
                )
            );

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            if ((
                    IndexSourceValue.StartsWith("KV", StringComparison.InvariantCultureIgnoreCase)
                    && IsNumeric(IndexSourceValue[2..])
                )
                ||
                (IndexSourceValue.StartsWith("XGV") && IsNumeric(IndexSourceValue[3]))
                ||
                (
                    IndexSourceValue.StartsWith("K", StringComparison.InvariantCultureIgnoreCase)
                    && IndexSourceValue.Trim().Length == 4
                ))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        HouseNumberSourceValue,
                        IndexSourceValue
                    )
                };
            }

            if (IndexSourceValue.StartsWith("GV", StringComparison.InvariantCultureIgnoreCase)
                && IsNumeric(IndexSourceValue[2..]))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        HouseNumberSourceValue,
                        $"0{IndexSourceValue[2..]}"
                    )
                };
            }

            throw new InvalidOperationException("Invalid use of matches");
        }
    }
}
