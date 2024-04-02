namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    using System;
    using System.Collections.Generic;

    public class SintNiklaas : HouseNumberBoxNumbersBase
    {
        public SintNiklaas(string nisCode, string sourceHouseNumber, NationalRegistryIndex index) : base(nisCode, sourceHouseNumber, index)
        { }

        public override bool IsMatch() =>
            NisCode == "46021" &&
            (
                Index.SourceValue!.StartsWith('B') && IsNumberGreaterThanZero(Index.SourceValue[1..])
                ||
                !Index.SourceValue!.StartsWith('B') && ContainsOnlyCapitalLetters(Index.SourceValue[0]) && Index.SourceValue[1..] == "000"
                ||
                !Index.SourceValue!.StartsWith('B') && IsNumberGreaterThanZero(Index.SourceValue[1..])
                ||
                ContainsOnlyCapitalLetters(Index.SourceValue[..2]) && Index.SourceValue[1] == 'B' && IsNumeric(Index.SourceValue[2..])
            );

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            if (Index.SourceValue!.StartsWith('B') && IsNumberGreaterThanZero(Index.SourceValue[1..]))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        SourceSourceHouseNumber,
                        $"{int.Parse(Index.SourceValue[1..])}"
                    )
                };
            }

            if (!Index.SourceValue!.StartsWith('B') && ContainsOnlyCapitalLetters(Index.SourceValue[0]) && Index.SourceValue[1..] == "000")
            {
                var bisNumber = Index.SourceValue[0];

                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        $"{SourceSourceHouseNumber}{bisNumber}",
                        null
                    )
                };
            }

            if (!Index.SourceValue!.StartsWith('B') && IsNumberGreaterThanZero(Index.SourceValue[1..]))
            {
                var bisNumber = Index.SourceValue[0];

                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        $"{SourceSourceHouseNumber}{bisNumber}",
                        $"{int.Parse(Index.SourceValue[1..])}".PadLeft(4, '0')
                    )
                };
            }

            if (ContainsOnlyCapitalLetters(Index.SourceValue[..2]) && Index.SourceValue[1] == 'B' && IsNumeric(Index.SourceValue[2..]))
            {
                var bisNumber = Index.SourceValue[0];

                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        $"{SourceSourceHouseNumber}{bisNumber}",
                        $"{int.Parse(Index.SourceValue[2..])}"
                    )
                };
            }

            throw new InvalidOperationException("Invalid use of matches");
        }
    }
}
