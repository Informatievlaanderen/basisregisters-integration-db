namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Lier : HouseNumberBoxNumbersBase
    {
        public Lier(string nisCode, string sourceHouseNumber, NationalRegistryIndex index) : base(nisCode, sourceHouseNumber, index)
        { }

        public override bool IsMatch() =>
            NisCode == "12021" &&
            (
                (Index.SourceValue!.StartsWith("B") && IsNumeric(Index.SourceValue[1..].Trim()))
                ||
                (ContainsOnlyCapitalLetters(Index.SourceValue[0].ToString()) && Index.SourceValue[1] == 'B' && IsNumeric(Index.SourceValue[2..].Trim()))
            );

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            if (Index.SourceValue!.StartsWith("B") && IsNumeric(Index.SourceValue[1..].Trim()))
            {
                return new List<HouseNumberWithBoxNumber>
                {
                    new HouseNumberWithBoxNumber(
                        SourceSourceHouseNumber,
                        int.Parse(Index.SourceValue[1..].Trim()).ToString())
                };
            }

            if (ContainsOnlyCapitalLetters(Index.SourceValue[0].ToString()) && Index.SourceValue[1] == 'B' && IsNumeric(Index.SourceValue[2..].Trim()))
            {
                return new List<HouseNumberWithBoxNumber>
                {
                    new HouseNumberWithBoxNumber(
                        $"{SourceSourceHouseNumber}{Index.SourceValue[0]}",
                        int.Parse(Index.SourceValue[2..].Trim()).ToString())
                };
            }

            throw new InvalidOperationException("Invalid use of matches");
        }
    }
}
