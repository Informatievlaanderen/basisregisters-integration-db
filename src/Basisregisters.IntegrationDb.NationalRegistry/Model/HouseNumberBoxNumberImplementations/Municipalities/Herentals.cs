namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    using System;
    using System.Collections.Generic;

    public class Herentals : MunicipalityHouseNumberBoxNumbersBase
    {
        public Herentals(string nisCode, string sourceHouseNumber, NationalRegistryIndex index) : base(nisCode, sourceHouseNumber, index)
        { }

        public override bool IsMatch() =>
            NisCode == "13011" &&
            (
                IndexSourceValue.Length == 4 && IsLetter(IndexSourceValue[0]) && IndexSourceValue[1..] == "000"
            );

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            if (IndexSourceValue.Length == 4 && IsLetter(IndexSourceValue[0]) && IndexSourceValue[1..] == "000")
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        HouseNumberSourceValue,
                        $"{IndexSourceValue[0]}"
                    )
                };
            }

            throw new InvalidOperationException("Invalid use of matches");
        }
    }
}
