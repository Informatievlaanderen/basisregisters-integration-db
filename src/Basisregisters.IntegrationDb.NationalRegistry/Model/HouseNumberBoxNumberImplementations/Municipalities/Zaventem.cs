namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    using System;
    using System.Collections.Generic;

    public class Zaventem : MunicipalityHouseNumberBoxNumbersBase
    {
        public Zaventem(string nisCode, string sourceHouseNumber, NationalRegistryIndex index) : base(nisCode, sourceHouseNumber, index)
        { }

        public override bool IsMatch() => NisCode == "23094" && (
            TrimmedIndexSourceValue.Length == 4 && char.ToUpper(TrimmedIndexSourceValue[0]) == 'X' && IsNumeric(TrimmedIndexSourceValue[1..])
        );

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            if (TrimmedIndexSourceValue.Length == 4 && char.ToUpper(TrimmedIndexSourceValue[0]) == 'X' && IsNumeric(TrimmedIndexSourceValue[1..]))
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
