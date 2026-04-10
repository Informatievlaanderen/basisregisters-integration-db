namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    using System;
    using System.Collections.Generic;

    public sealed class Linkebeek : MunicipalityHouseNumberBoxNumbersBase
    {
        public Linkebeek(string nisCode, string sourceHouseNumber, NationalRegistryIndex index) : base(nisCode, sourceHouseNumber, index)
        { }

        public override bool IsMatch() => NisCode == "23100" && IndexSourceValue.Length == 4 &&
                                          char.ToUpperInvariant(IndexSourceValue[0]) == 'B' && IsNumeric(IndexSourceValue[1..]);

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            if (char.ToUpperInvariant(IndexSourceValue[0]) == 'B' && IsNumeric(IndexSourceValue[1..]))
            {
                return
                [
                    new HouseNumberWithBoxNumber(
                        HouseNumberSourceValue,
                        Index.Right!.TrimStart('0')
                    )
                ];
            }

            throw new InvalidOperationException("Invalid use of matches");
        }
    }
}
