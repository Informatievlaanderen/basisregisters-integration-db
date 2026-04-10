namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    using System;
    using System.Collections.Generic;

    public sealed class Leopoldsburg : MunicipalityHouseNumberBoxNumbersBase
    {
        public Leopoldsburg(string nisCode, string sourceHouseNumber, NationalRegistryIndex index) : base(nisCode, sourceHouseNumber, index)
        { }

        public override bool IsMatch() => NisCode == "71034" && IndexSourceValue.Length == 4 &&
                                          (
                                              ((IndexSourceValue[0] == 'A' || IndexSourceValue[0] == 'B') && IndexSourceValue[1..] == "000")
                                              ||
                                              (IndexSourceValue[0] == 'B' && IsNumberGreaterThanZero(IndexSourceValue[1..]))
                                          );

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            if ((IndexSourceValue[0] == 'A' || IndexSourceValue[0] == 'B') && IndexSourceValue[1..] == "000")
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        HouseNumberSourceValue,
                        IndexSourceValue[0].ToString()
                    )
                };
            }

            if (IndexSourceValue[0] == 'B' && IsNumberGreaterThanZero(IndexSourceValue[1..]))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        HouseNumberSourceValue,
                        IndexSourceValue[1..].TrimStart('0')
                    )
                };
            }

            throw new InvalidOperationException("Invalid use of matches");
        }
    }
}
