namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    using System;
    using System.Collections.Generic;

    public class Beerse : HouseNumberBoxNumbersBase
    {
        public Beerse(string nisCode, string sourceHouseNumber, NationalRegistryIndex index) : base(nisCode, sourceHouseNumber, index)
        { }

        public override bool IsMatch() =>
            NisCode == "13004" &&
            (
                Index.SourceValue!.StartsWith('B') && IsNumeric(Index.SourceValue![1..])
            );

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            if (Index.SourceValue!.StartsWith('B') && IsNumeric(Index.SourceValue![1..]))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        SourceSourceHouseNumber,
                        Index.SourceValue!.Trim()
                    )
                };
            }

            throw new InvalidOperationException("Invalid use of matches");
        }
    }
}
