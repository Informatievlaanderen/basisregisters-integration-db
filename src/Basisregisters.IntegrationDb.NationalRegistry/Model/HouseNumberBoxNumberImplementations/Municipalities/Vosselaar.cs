namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    using System;
    using System.Collections.Generic;

    public class Vosselaar : HouseNumberBoxNumbersBase
    {
        public Vosselaar(string nisCode, string sourceHouseNumber, NationalRegistryIndex index) : base(nisCode, sourceHouseNumber, index)
        { }

        public override bool IsMatch() => NisCode == "13046" && (
            Index.SourceValue!.StartsWith("b", StringComparison.InvariantCultureIgnoreCase) && IsNumeric(Index.SourceValue![1..])
        );

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            if (Index.SourceValue!.StartsWith("b", StringComparison.InvariantCultureIgnoreCase) && IsNumeric(Index.SourceValue![1..]))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        SourceSourceHouseNumber,
                        int.Parse(Index.SourceValue![1..]).ToString()
                    )
                };
            }

            throw new InvalidOperationException("Invalid use of matches");
        }
    }
}
