namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    using System;
    using System.Collections.Generic;

    public class Antwerpen : MunicipalityHouseNumberBoxNumbersBase
    {
        public Antwerpen(string nisCode, string sourceHouseNumber, NationalRegistryIndex index) : base(nisCode, sourceHouseNumber, index)
        { }

        public override bool IsMatch()
        {
            var trimmed = IndexSourceValue.Trim();

            return NisCode == "11002" &&
                   (
                       (trimmed.Length == 2 && ContainsOnlyCapitalLetters(trimmed[0]) && IsNumeric(trimmed[1]))
                       ||
                       IsNumeric(trimmed)
                   );
        }

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            var trimmed = IndexSourceValue.Trim();

            if (trimmed.Length == 2 && ContainsOnlyCapitalLetters(trimmed[0]) && IsNumeric(trimmed[1]))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        HouseNumberSourceValue,
                        trimmed
                    )
                };
            }

            if (IsNumeric(trimmed))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        HouseNumberSourceValue,
                        $"{int.Parse(trimmed)}"
                    )
                };
            }

            throw new InvalidOperationException("Invalid use of matches");
        }
    }
}
