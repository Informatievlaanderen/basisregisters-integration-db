namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class SintGillisWaas : HouseNumberBoxNumbersBase
    {
        public SintGillisWaas(string nisCode, string sourceHouseNumber, NationalRegistryIndex index) : base(nisCode, sourceHouseNumber, index)
        { }

        public override bool IsMatch() =>
            NisCode == "46020" &&
            (
                (IsLetter(Index.SourceValue![0]) && Index.SourceValue[1..] == "000")
                ||
                (char.ToUpper(Index.SourceValue[0]) == 'B' && IsNumberGreaterThanZero(Index.SourceValue[1..]))
                ||
                (IsLetter(Index.SourceValue[0]) && Index.SourceValue.Contains('/') && IsNumeric(Index.SourceValue[3]))
                ||
                (IsLetter(Index.SourceValue[0]) && char.ToUpper(Index.SourceValue[1]) == 'B' && IsNumeric(Index.SourceValue[2..]))
            );

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            if (IsLetter(Index.SourceValue![0]) && Index.SourceValue[1..] == "000")
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        $"{SourceSourceHouseNumber}{Index.SourceValue[0]}",
                        null
                    )
                };
            }

            if (char.ToUpper(Index.SourceValue[0]) == 'B' && IsNumberGreaterThanZero(Index.SourceValue[1..]))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        SourceSourceHouseNumber,
                        int.Parse(Index.SourceValue[1..]).ToString()
                    )
                };
            }

            if (IsLetter(Index.SourceValue[0]) && Index.SourceValue.Contains('/') && IsNumeric(Index.SourceValue[3]))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        $"{SourceSourceHouseNumber}{Index.SourceValue[0]}",
                        int.Parse(string.Join(string.Empty, Index.SourceValue.Split('/').Last().Where(char.IsDigit))).ToString()
                    )
                };
            }

            if (IsLetter(Index.SourceValue[0]) && char.ToUpper(Index.SourceValue[1]) == 'B' && IsNumeric(Index.SourceValue[2..]))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        $"{SourceSourceHouseNumber}{Index.SourceValue[0]}",
                        int.Parse(Index.SourceValue[2..]).ToString()
                    )
                };
            }

            throw new InvalidOperationException("Invalid use of matches");
        }
    }
}
