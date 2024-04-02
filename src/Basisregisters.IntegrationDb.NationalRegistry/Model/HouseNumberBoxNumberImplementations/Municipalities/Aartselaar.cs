namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    using System;
    using System.Collections.Generic;

    public class Aartselaar : HouseNumberBoxNumbersBase
    {
        public Aartselaar(string nisCode, string sourceHouseNumber, NationalRegistryIndex index) : base(nisCode, sourceHouseNumber, index)
        { }

        public override bool IsMatch() =>
            NisCode == "11001" &&
            (
                ContainsOnlyCapitalLetters(Index.SourceValue![0]) && IsNumeric(Index.SourceValue[1..])
            );

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            if (ContainsOnlyCapitalLetters(Index.SourceValue![0]) && IsNumeric(Index.SourceValue[1..]))
            {
                return new []
                {
                    new HouseNumberWithBoxNumber(
                        SourceSourceHouseNumber,
                        $"{Index.SourceValue[0]}{Index.SourceValue[1..].TrimStart('0')}"
                    )
                };
            }

            throw new InvalidOperationException("Invalid use of matches");
        }
    }
}
