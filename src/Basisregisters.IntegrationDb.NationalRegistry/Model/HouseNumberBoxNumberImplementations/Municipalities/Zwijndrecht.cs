namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    using System;
    using System.Collections.Generic;

    public class Zwijndrecht : MunicipalityHouseNumberBoxNumbersBase
    {
        public Zwijndrecht(string nisCode, string sourceHouseNumber, NationalRegistryIndex index) : base(nisCode, sourceHouseNumber, index)
        { }

        public override bool IsMatch() =>
            NisCode == "11056" &&
            (
                char.ToLower(IndexSourceValue[0]) == 'b' && IsNumeric(IndexSourceValue[1..])
                ||
                IsLetter(IndexSourceValue[0]) && IsNumeric(IndexSourceValue[1..])
                ||
                IndexSourceValue.StartsWith("Glv", StringComparison.InvariantCultureIgnoreCase)
            );

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            if (char.ToLower(IndexSourceValue[0]) == 'b' && IsNumeric(IndexSourceValue[1..]))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        SourceSourceHouseNumber,
                        int.Parse(IndexSourceValue[1..]).ToString()
                    )
                };
            }

            if (char.IsLetter(IndexSourceValue[0]) && IsNumeric(IndexSourceValue[1..]))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        SourceSourceHouseNumber,
                        $"{IndexSourceValue[0]}{int.Parse(IndexSourceValue[1..])}"
                    )
                };
            }

            if (IndexSourceValue.StartsWith("Glv", StringComparison.InvariantCultureIgnoreCase))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        SourceSourceHouseNumber,
                        "0.0"
                    )
                };
            }

            throw new InvalidOperationException("Invalid use of matches");
        }
    }
}
