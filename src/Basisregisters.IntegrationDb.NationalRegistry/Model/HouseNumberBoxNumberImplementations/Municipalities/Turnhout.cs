namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    using System;
    using System.Collections.Generic;

    public class Turnhout : MunicipalityHouseNumberBoxNumbersBase
    {
        public Turnhout(string nisCode, string sourceHouseNumber, NationalRegistryIndex index) : base(nisCode, sourceHouseNumber, index)
        { }

        public override bool IsMatch() =>
            NisCode == "13040" &&
            (
                IndexSourceValue.StartsWith('b')
                ||
                IndexSourceValue.StartsWith('0') && IsNumberGreaterThanZero(IndexSourceValue)
                ||
                ContainsOnlyCapitalLetters(IndexSourceValue[..1])
            );

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            if (IndexSourceValue.StartsWith('b'))
            {
                return new List<HouseNumberWithBoxNumber>
                {
                    new HouseNumberWithBoxNumber(
                        HouseNumberSourceValue,
                        IndexSourceValue.Replace("b00", string.Empty).Replace("b0", string.Empty))
                };
            }

            if (IndexSourceValue.StartsWith('0') && IsNumberGreaterThanZero(IndexSourceValue))
            {
                return new List<HouseNumberWithBoxNumber>
                {
                    new HouseNumberWithBoxNumber(
                        $"{HouseNumberSourceValue}_{IndexSourceValue.TrimStart('0')}",
                        null)
                };
            }

            if (ContainsOnlyCapitalLetters(IndexSourceValue.Substring(0, 1)))
            {
                var boxNumber = $"{IndexSourceValue[0]}{IndexSourceValue[1..].TrimStart('0')}";

                return new List<HouseNumberWithBoxNumber>
                {
                    new HouseNumberWithBoxNumber(
                        HouseNumberSourceValue,
                        boxNumber)
                };
            }

            throw new InvalidOperationException("Invalid use of matches");
        }
    }
}
