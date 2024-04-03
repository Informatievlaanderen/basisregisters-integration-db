namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    using System;
    using System.Collections.Generic;

    public class SintNiklaas : MunicipalityHouseNumberBoxNumbersBase
    {
        public SintNiklaas(string nisCode, string sourceHouseNumber, NationalRegistryIndex index) : base(nisCode, sourceHouseNumber, index)
        { }

        public override bool IsMatch() =>
            NisCode == "46021" &&
            (
                IndexSourceValue!.StartsWith('B') && IsNumberGreaterThanZero(IndexSourceValue[1..])
                ||
                !IndexSourceValue!.StartsWith('B') && ContainsOnlyCapitalLetters(IndexSourceValue[0]) && IndexSourceValue[1..] == "000"
                ||
                !IndexSourceValue!.StartsWith('B') && IsNumberGreaterThanZero(IndexSourceValue[1..])
                ||
                ContainsOnlyCapitalLetters(IndexSourceValue[..2]) && IndexSourceValue[1] == 'B' && IsNumeric(IndexSourceValue[2..])
            );

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            if (IndexSourceValue!.StartsWith('B') && IsNumberGreaterThanZero(IndexSourceValue[1..]))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        SourceSourceHouseNumber,
                        $"{int.Parse(IndexSourceValue[1..])}"
                    )
                };
            }

            if (!IndexSourceValue!.StartsWith('B') && ContainsOnlyCapitalLetters(IndexSourceValue[0]) && IndexSourceValue[1..] == "000")
            {
                var bisNumber = IndexSourceValue[0];

                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        $"{SourceSourceHouseNumber}{bisNumber}",
                        null
                    )
                };
            }

            if (!IndexSourceValue!.StartsWith('B') && IsNumberGreaterThanZero(IndexSourceValue[1..]))
            {
                var bisNumber = IndexSourceValue[0];

                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        $"{SourceSourceHouseNumber}{bisNumber}",
                        $"{int.Parse(IndexSourceValue[1..])}".PadLeft(4, '0')
                    )
                };
            }

            if (ContainsOnlyCapitalLetters(IndexSourceValue[..2]) && IndexSourceValue[1] == 'B' && IsNumeric(IndexSourceValue[2..]))
            {
                var bisNumber = IndexSourceValue[0];

                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        $"{SourceSourceHouseNumber}{bisNumber}",
                        $"{int.Parse(IndexSourceValue[2..])}"
                    )
                };
            }

            throw new InvalidOperationException("Invalid use of matches");
        }
    }
}
