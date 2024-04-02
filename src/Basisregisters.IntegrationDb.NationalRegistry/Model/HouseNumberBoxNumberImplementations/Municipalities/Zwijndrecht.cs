namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    using System;
    using System.Collections.Generic;

    public class Zwijndrecht : HouseNumberBoxNumbersBase
    {
        public Zwijndrecht(string nisCode, string sourceHouseNumber, NationalRegistryIndex index) : base(nisCode, sourceHouseNumber, index)
        { }

        public override bool IsMatch() =>
            NisCode == "11056" &&
            (
                char.ToLower(Index.SourceValue![0]) == 'b' && IsNumeric(Index.SourceValue[1..])
                ||
                IsLetter(Index.SourceValue![0]) && IsNumeric(Index.SourceValue[1..])
                ||
                Index.SourceValue.StartsWith("Glv", StringComparison.InvariantCultureIgnoreCase)
            );

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            if (char.ToLower(Index.SourceValue![0]) == 'b' && IsNumeric(Index.SourceValue[1..]))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        SourceSourceHouseNumber,
                        int.Parse(Index.SourceValue[1..]).ToString()
                    )
                };
            }

            if (char.IsLetter(Index.SourceValue![0]) && IsNumeric(Index.SourceValue[1..]))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        SourceSourceHouseNumber,
                        $"{Index.SourceValue[0]}{int.Parse(Index.SourceValue[1..])}"
                    )
                };
            }

            if (Index.SourceValue.StartsWith("Glv", StringComparison.InvariantCultureIgnoreCase))
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
