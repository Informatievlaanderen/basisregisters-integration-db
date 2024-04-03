namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    using System;
    using System.Collections.Generic;

    public class Houthalen : MunicipalityHouseNumberBoxNumbersBase
    {
        public Houthalen(string nisCode, string sourceHouseNumber, NationalRegistryIndex index) : base(nisCode, sourceHouseNumber, index)
        { }

        public override bool IsMatch() =>
            NisCode == "72039" &&
            (
                IndexSourceValue.Contains('-')
                ||
                IsLetter(IndexSourceValue[0]) && IndexSourceValue[1..] == "000"
                ||
                IndexSourceValue[0] == '0' && IsNumeric(IndexSourceValue)
            );

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            if (IndexSourceValue.Contains('-'))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        SourceSourceHouseNumber,
                        IndexSourceValue.Trim()
                    )
                };
            }

            if (IsLetter(IndexSourceValue[0]) && IndexSourceValue[1..] == "000")
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

            if (IndexSourceValue[0] == '0' && IsNumeric(IndexSourceValue))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        SourceSourceHouseNumber,
                        $"{int.Parse(IndexSourceValue)}"
                    )
                };
            }

            throw new InvalidOperationException("Invalid use of matches");
        }
    }
}
