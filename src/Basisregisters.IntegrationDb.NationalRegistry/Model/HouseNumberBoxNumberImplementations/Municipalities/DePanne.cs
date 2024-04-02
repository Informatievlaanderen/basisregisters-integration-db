namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    using System;
    using System.Collections.Generic;

    public class DePanne : HouseNumberBoxNumbersBase
    {
        public DePanne(string nisCode, string sourceHouseNumber, NationalRegistryIndex index) : base(nisCode, sourceHouseNumber, index)
        { }

        public override bool IsMatch() =>
            NisCode == "38008" &&
            (
                IsLetter(Index.SourceValue![0])
                ||
                IsNumeric(Index.SourceValue!)
            );

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            if (IsLetter(Index.SourceValue![0]))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        SourceSourceHouseNumber,
                        Index.SourceValue!.Trim()
                    )
                };
            }

            if (IsNumeric(Index.SourceValue!))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        $"{SourceSourceHouseNumber}_{int.Parse(Index.SourceValue!)}",
                        null
                    )
                };
            }

            throw new InvalidOperationException("Invalid use of matches");
        }
    }
}
