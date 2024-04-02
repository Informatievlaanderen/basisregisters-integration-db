namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    using System;
    using System.Collections.Generic;

    public class Houthalen : HouseNumberBoxNumbersBase
    {
        public Houthalen(string nisCode, string sourceHouseNumber, NationalRegistryIndex index) : base(nisCode, sourceHouseNumber, index)
        { }

        public override bool IsMatch() =>
            NisCode == "72039" &&
            (
                Index.SourceValue!.Contains('-')
                ||
                IsLetter(Index.SourceValue[0]) && Index.SourceValue[1..] == "000"
                ||
                Index.SourceValue[0] == '0' && IsNumeric(Index.SourceValue)
            );

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            if (Index.SourceValue!.Contains('-'))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        SourceSourceHouseNumber,
                        Index.SourceValue.Trim()
                    )
                };
            }

            if (IsLetter(Index.SourceValue[0]) && Index.SourceValue[1..] == "000")
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

            if (Index.SourceValue[0] == '0' && IsNumeric(Index.SourceValue))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        SourceSourceHouseNumber,
                        $"{int.Parse(Index.SourceValue)}"
                    )
                };
            }

            throw new InvalidOperationException("Invalid use of matches");
        }
    }
}
