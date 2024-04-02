namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    using System;
    using System.Collections.Generic;

    public class Aalter : HouseNumberBoxNumbersBase
    {
        public Aalter(string nisCode, string sourceHouseNumber, NationalRegistryIndex index) : base(nisCode, sourceHouseNumber, index)
        { }

        public override bool IsMatch() =>
            NisCode == "44084" &&
            (
                char.ToUpper(Index.SourceValue![2]) == 'O'
                ||
                (Index.SourceValue.StartsWith('0') && IsLetter(Index.SourceValue[3]))
                ||
                (IsLetter(Index.SourceValue[0]) && Index.SourceValue[1..] == "000")
            );

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            if (char.ToUpper(Index.SourceValue![2]) == 'O')
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        SourceSourceHouseNumber,
                        Index.SourceValue!.Trim()
                    )
                };
            }

            if (Index.SourceValue.StartsWith('0') && IsLetter(Index.SourceValue[3]))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        SourceSourceHouseNumber,
                        Index.SourceValue!.Trim().TrimStart('0')
                    )
                };
            }

            if (IsLetter(Index.SourceValue[0]) && Index.SourceValue[1..] == "000")
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        $"{SourceSourceHouseNumber}{Index.SourceValue[0]}",
                        null
                    )
                };
            }

            throw new InvalidOperationException("Invalid use of matches");
        }
    }
}
