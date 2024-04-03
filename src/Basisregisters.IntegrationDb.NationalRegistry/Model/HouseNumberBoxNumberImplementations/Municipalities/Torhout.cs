namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    using System;
    using System.Collections.Generic;

    public class Torhout : MunicipalityHouseNumberBoxNumbersBase
    {
        public Torhout(string nisCode, string sourceHouseNumber, NationalRegistryIndex index) : base(nisCode, sourceHouseNumber, index)
        { }

        public override bool IsMatch() =>
            NisCode == "31033" &&
            (
                IndexSourceValue.StartsWith("V", StringComparison.InvariantCultureIgnoreCase) && IsNumeric(IndexSourceValue[1..])
                ||
                IndexSourceValue.StartsWith("AV", StringComparison.InvariantCultureIgnoreCase) && IsNumeric(IndexSourceValue[2..])
            );

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            if (IndexSourceValue.StartsWith("V", StringComparison.InvariantCultureIgnoreCase) && IsNumeric(IndexSourceValue[1..]))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        SourceSourceHouseNumber,
                        $"{IndexSourceValue[0]}{int.Parse(IndexSourceValue[1..])}"
                    )
                };
            }

            if (IndexSourceValue.StartsWith("AV", StringComparison.InvariantCultureIgnoreCase) && IsNumeric(IndexSourceValue[2..]))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        SourceSourceHouseNumber,
                        IndexSourceValue.Trim()
                    )
                };
            }

            throw new InvalidOperationException("Invalid use of matches");
        }
    }
}
