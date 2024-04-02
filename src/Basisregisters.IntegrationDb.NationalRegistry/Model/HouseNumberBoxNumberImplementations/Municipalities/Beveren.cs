namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    using System;
    using System.Collections.Generic;

    public class Beveren : HouseNumberBoxNumbersBase
    {
        public Beveren(string nisCode, string sourceHouseNumber, NationalRegistryIndex index) : base(nisCode, sourceHouseNumber, index)
        { }

        public override bool IsMatch() =>
            NisCode == "46003" &&
            (
                (Index.SourceValue!.StartsWith("b", StringComparison.InvariantCultureIgnoreCase) && IsNumeric(Index.SourceValue![1..]))
                ||
                (!Index.SourceValue!.StartsWith("b", StringComparison.InvariantCultureIgnoreCase) && char.ToLower(Index.SourceValue![1]) == 'b' && IsNumeric(Index.SourceValue![2..]))
                ||
                (Index.SourceValue!.StartsWith("000") && char.IsDigit(Index.SourceValue![3]))
            );

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            if (Index.SourceValue!.StartsWith("b", StringComparison.InvariantCultureIgnoreCase) && IsNumeric(Index.SourceValue![1..]))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        SourceSourceHouseNumber,
                        int.Parse(Index.SourceValue![1..]).ToString().PadLeft(3, '0')
                    )
                };
            }

            if (!Index.SourceValue!.StartsWith("b", StringComparison.InvariantCultureIgnoreCase) && char.ToLower(Index.SourceValue![1]) == 'b' && IsNumeric(Index.SourceValue![2..]))
            {
                var bisNumber = Index.SourceValue![0];
                var houseNumber = char.IsDigit(bisNumber) ? $"{SourceSourceHouseNumber}_{bisNumber}" : $"{SourceSourceHouseNumber}{bisNumber}";

                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        houseNumber,
                        int.Parse(Index.SourceValue![2..]).ToString().PadLeft(3, '0')
                    ),
                    new HouseNumberWithBoxNumber(
                        houseNumber,
                        int.Parse(Index.SourceValue![2..]).ToString().PadLeft(2, '0')
                    ),
                };
            }

            if (Index.SourceValue!.StartsWith("000") && char.IsDigit(Index.SourceValue![3]))
            {
                var bisNumber = Index.SourceValue![3];
                var houseNumber = $"{SourceSourceHouseNumber}_{bisNumber}";

                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        houseNumber,
                        null
                    )
                };
            }

            throw new InvalidOperationException("Invalid use of matches");
        }
    }
}
