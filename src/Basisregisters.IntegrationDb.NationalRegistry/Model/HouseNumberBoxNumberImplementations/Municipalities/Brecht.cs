namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Brecht : HouseNumberBoxNumbersBase
    {
        public Brecht(string nisCode, string sourceHouseNumber, NationalRegistryIndex index) : base(nisCode, sourceHouseNumber, index)
        { }

        public override bool IsMatch() =>
            NisCode == "11009" &&
            (
                IsLetter(Index.SourceValue![0]) && Index.SourceValue.Contains('/')
                ||
                Index.SourceValue.StartsWith("GV", StringComparison.InvariantCultureIgnoreCase) && IsNumeric(Index.SourceValue[2..])
                ||
                Index.SourceValue[0] == '0' && IsLetter(Index.SourceValue[3])
            );

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            if (IsLetter(Index.SourceValue![0]) && Index.SourceValue.Contains('/'))
            {
                var parts = Index.SourceValue.Split('/');

                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        $"{SourceSourceHouseNumber}{parts.First()}",
                        parts.Last()
                    )
                };
            }

            if (Index.SourceValue.StartsWith("GV", StringComparison.InvariantCultureIgnoreCase) && IsNumeric(Index.SourceValue[2..]))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        SourceSourceHouseNumber,
                        Index.SourceValue.Trim()
                    )
                };
            }

            if (Index.SourceValue[0] == '0' && IsLetter(Index.SourceValue[3]))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        SourceSourceHouseNumber,
                        Index.SourceValue.TrimStart('0')
                    )
                };
            }

            throw new InvalidOperationException("Invalid use of matches");
        }
    }
}
