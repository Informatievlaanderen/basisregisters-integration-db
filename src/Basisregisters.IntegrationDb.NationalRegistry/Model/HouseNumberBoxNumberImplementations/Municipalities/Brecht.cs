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
            );

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            if (IsLetter(IndexSourceValue[0]) && IndexSourceValue.Contains('/'))
            {
                var parts = IndexSourceValue.Split('/');

                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        $"{SourceSourceHouseNumber}{parts.First()}",
                        parts.Last()
                    )
                };
            }

            if (IndexSourceValue.StartsWith("GV", StringComparison.InvariantCultureIgnoreCase) && IsNumeric(IndexSourceValue[2..]))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        SourceSourceHouseNumber,
                        IndexSourceValue.Trim()
                    )
                };
            }

            if (IndexSourceValue[0] == '0' && IsLetter(IndexSourceValue[3]))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        SourceSourceHouseNumber,
                        IndexSourceValue.TrimStart('0')
                    )
                };
            }

            throw new InvalidOperationException("Invalid use of matches");
        }
    }
}
