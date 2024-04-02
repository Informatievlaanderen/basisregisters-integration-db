namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    using System;
    using System.Collections.Generic;

    public class Torhout : HouseNumberBoxNumbersBase
    {
        public Torhout(string nisCode, string sourceHouseNumber, NationalRegistryIndex index) : base(nisCode, sourceHouseNumber, index)
        { }

        public override bool IsMatch() =>
            NisCode == "31033" &&
            (
                Index.SourceValue!.StartsWith("V", StringComparison.InvariantCultureIgnoreCase) && IsNumeric(Index.SourceValue[1..])
                ||
                Index.SourceValue!.StartsWith("AV", StringComparison.InvariantCultureIgnoreCase) && IsNumeric(Index.SourceValue[2..])
            );

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            if (Index.SourceValue!.StartsWith("V", StringComparison.InvariantCultureIgnoreCase) && IsNumeric(Index.SourceValue[1..]))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        SourceSourceHouseNumber,
                        $"{Index.SourceValue[0]}{int.Parse(Index.SourceValue[1..])}"
                    )
                };
            }

            if (Index.SourceValue!.StartsWith("AV", StringComparison.InvariantCultureIgnoreCase) && IsNumeric(Index.SourceValue[2..]))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        SourceSourceHouseNumber,
                        Index.SourceValue.Trim()
                    )
                };
            }

            throw new InvalidOperationException("Invalid use of matches");
        }
    }
}
