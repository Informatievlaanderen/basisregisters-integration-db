namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    using System;
    using System.Collections.Generic;

    public class Aartselaar : MunicipalityHouseNumberBoxNumbersBase
    {
        public Aartselaar(string nisCode, string sourceHouseNumber, NationalRegistryIndex index) : base(nisCode, sourceHouseNumber, index)
        { }

        public override bool IsMatch() =>
            NisCode == "11001" &&
            (
                ContainsOnlyCapitalLetters(IndexSourceValue[0]) && IsNumeric(IndexSourceValue[1..])
            );

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            if (ContainsOnlyCapitalLetters(IndexSourceValue[0]) && IsNumeric(IndexSourceValue[1..]))
            {
                return new []
                {
                    new HouseNumberWithBoxNumber(
                        HouseNumberSourceValue,
                        $"{IndexSourceValue[0]}{IndexSourceValue[1..].TrimStart('0')}"
                    )
                };
            }

            throw new InvalidOperationException("Invalid use of matches");
        }
    }
}
