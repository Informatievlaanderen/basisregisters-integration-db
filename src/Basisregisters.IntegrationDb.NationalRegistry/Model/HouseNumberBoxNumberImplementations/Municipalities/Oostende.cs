namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    using System;
    using System.Collections.Generic;

    public class Oostende : MunicipalityHouseNumberBoxNumbersBase
    {
        public Oostende(string nisCode, string sourceHouseNumber, NationalRegistryIndex index) : base(nisCode, sourceHouseNumber, index)
        { }

        public override bool IsMatch() =>
            NisCode == "35013" &&
            (
                TrimmedIndexSourceValue.Length == 4 && IsNumeric(TrimmedIndexSourceValue)
            );

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            if (TrimmedIndexSourceValue.Length == 4 && IsNumeric(TrimmedIndexSourceValue))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        HouseNumberSourceValue,
                        TrimmedIndexSourceValue
                    )
                };
            }

            throw new InvalidOperationException("Invalid use of matches");
        }
    }
}
