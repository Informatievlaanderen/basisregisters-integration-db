namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    using System;
    using System.Collections.Generic;

    public class Turnhout : HouseNumberBoxNumbersBase
    {
        public Turnhout(string nisCode, string sourceHouseNumber, NationalRegistryIndex index) : base(nisCode, sourceHouseNumber, index)
        { }

        public override bool IsMatch() =>
            NisCode == "13040" &&
            (
                Index.SourceValue!.StartsWith('b')
                ||
                Index.SourceValue!.StartsWith('0') && IsNumberGreaterThanZero(Index.SourceValue)
                ||
                ContainsOnlyCapitalLetters(Index.SourceValue![..1])
            );

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            if (Index.SourceValue!.StartsWith('b'))
            {
                return new List<HouseNumberWithBoxNumber>
                {
                    new HouseNumberWithBoxNumber(
                        SourceSourceHouseNumber,
                        Index.SourceValue.Replace("b00", string.Empty).Replace("b0", string.Empty))
                };
            }

            if (Index.SourceValue!.StartsWith('0') && IsNumberGreaterThanZero(Index.SourceValue))
            {
                return new List<HouseNumberWithBoxNumber>
                {
                    new HouseNumberWithBoxNumber(
                        $"{SourceSourceHouseNumber}_{Index.SourceValue!.TrimStart('0')}",
                        null)
                };
            }

            if (ContainsOnlyCapitalLetters(Index.SourceValue!.Substring(0, 1)))
            {
                var boxNumber = $"{Index.SourceValue[0]}{Index.SourceValue[1..].TrimStart('0')}";

                return new List<HouseNumberWithBoxNumber>
                {
                    new HouseNumberWithBoxNumber(
                        SourceSourceHouseNumber,
                        boxNumber)
                };
            }

            throw new InvalidOperationException("Invalid use of matches");
        }
    }
}
