namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    using System;
    using System.Collections.Generic;

    public class Aalter : MunicipalityHouseNumberBoxNumbersBase
    {
        public Aalter(string nisCode, string sourceHouseNumber, NationalRegistryIndex index) : base(nisCode, sourceHouseNumber, index)
        { }

        public override bool IsMatch() =>
            NisCode == "44084" &&
            (
                char.ToUpper(IndexSourceValue![2]) == 'O'
                ||
                (IndexSourceValue.StartsWith('0') && IsLetter(IndexSourceValue[3]))
                ||
                (IsLetter(IndexSourceValue[0]) && IndexSourceValue[1..] == "000")
            );

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            if (char.ToUpper(IndexSourceValue![2]) == 'O')
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        SourceSourceHouseNumber,
                        IndexSourceValue!.Trim()
                    )
                };
            }

            if (IndexSourceValue.StartsWith('0') && IsLetter(IndexSourceValue[3]))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        SourceSourceHouseNumber,
                        IndexSourceValue!.Trim().TrimStart('0')
                    )
                };
            }

            if (IsLetter(IndexSourceValue[0]) && IndexSourceValue[1..] == "000")
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        $"{SourceSourceHouseNumber}{IndexSourceValue[0]}",
                        null
                    )
                };
            }

            throw new InvalidOperationException("Invalid use of matches");
        }
    }
}
