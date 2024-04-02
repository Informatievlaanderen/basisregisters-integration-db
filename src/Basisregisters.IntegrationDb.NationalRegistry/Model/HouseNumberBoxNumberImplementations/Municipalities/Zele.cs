namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    using System;
    using System.Collections.Generic;

    public class Zele : HouseNumberBoxNumbersBase
    {
        public Zele(string nisCode, string sourceHouseNumber, NationalRegistryIndex index) : base(nisCode, sourceHouseNumber, index)
        { }

        public override bool IsMatch() =>
            NisCode == "71020" &&
            (
                Index.SourceValue!.StartsWith("b", StringComparison.InvariantCultureIgnoreCase) && IsNumeric(Index.SourceValue[1..])
                ||
                Index.SourceValue.Contains('.')
            );

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            if (Index.SourceValue!.StartsWith("b", StringComparison.InvariantCultureIgnoreCase) && IsNumeric(Index.SourceValue[1..]))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        SourceSourceHouseNumber,
                        Index.SourceValue.Trim()
                    )
                };
            }

            if (Index.SourceValue.Contains('.'))
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
