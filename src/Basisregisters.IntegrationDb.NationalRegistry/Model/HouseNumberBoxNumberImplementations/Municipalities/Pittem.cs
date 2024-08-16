namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Pittem : MunicipalityHouseNumberBoxNumbersBase
    {
        public Pittem(string nisCode, string sourceHouseNumber, NationalRegistryIndex index) : base(nisCode, sourceHouseNumber, index)
        { }

        public override bool IsMatch() => NisCode == "37011" && (
            (TrimmedIndexSourceValue == "BIS")
            ||
            (TrimmedIndexSourceValue.Length >= 3 && IsNumeric(TrimmedIndexSourceValue[0]) && TrimmedIndexSourceValue[1] == '/' && ContainsOnlyLetters(TrimmedIndexSourceValue[2..]))
        );

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            if (TrimmedIndexSourceValue == "BIS")
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        $"{HouseNumberSourceValue}B",
                        null
                    )
                };
            }

            if (TrimmedIndexSourceValue.Length >= 3 && IsNumeric(TrimmedIndexSourceValue[0]) && TrimmedIndexSourceValue[1] == '/' && ContainsOnlyLetters(TrimmedIndexSourceValue[2..]))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        HouseNumberSourceValue,
                        TrimmedIndexSourceValue.Replace("/", string.Empty)
                    )
                };
            }

            throw new InvalidOperationException("Invalid use of matches");
        }
    }
}
