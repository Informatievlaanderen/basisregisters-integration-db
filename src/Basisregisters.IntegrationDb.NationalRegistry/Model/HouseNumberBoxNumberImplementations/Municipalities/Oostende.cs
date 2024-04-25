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
                IsNumeric(IndexSourceValue)
            );

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            if (IsNumeric(IndexSourceValue))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        HouseNumberSourceValue,
                        IndexSourceValue
                    )
                };
            }

            throw new InvalidOperationException("Invalid use of matches");
        }
    }
}
