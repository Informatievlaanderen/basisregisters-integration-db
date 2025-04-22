namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    using System;
    using System.Collections.Generic;

    public class BeverenKruibekeZwijndrecht : MunicipalityHouseNumberBoxNumbersBase
    {
        public BeverenKruibekeZwijndrecht(string nisCode, string sourceHouseNumber, NationalRegistryIndex index) : base(nisCode, sourceHouseNumber, index)
        { }

        public override bool IsMatch() =>
            NisCode == "46030" &&
            (
                (IndexSourceValue.StartsWith("b", StringComparison.InvariantCultureIgnoreCase) && IsNumeric(IndexSourceValue[1..]))
                ||
                (!IndexSourceValue.StartsWith("b", StringComparison.InvariantCultureIgnoreCase) && char.ToLower(IndexSourceValue[1]) == 'b' && IsNumeric(IndexSourceValue[2..]))
                ||
                (IndexSourceValue.StartsWith("000") && char.IsDigit(IndexSourceValue[3]))
                ||
                IsLetter(IndexSourceValue[0]) && IsNumeric(IndexSourceValue[1..])
                ||
                IndexSourceValue.StartsWith("Glv", StringComparison.InvariantCultureIgnoreCase)
            );

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            if (IndexSourceValue.StartsWith("b", StringComparison.InvariantCultureIgnoreCase) && IsNumeric(IndexSourceValue[1..]))
            {
                return
                [
                    new HouseNumberWithBoxNumber(
                        HouseNumberSourceValue,
                        int.Parse(IndexSourceValue[1..]).ToString().PadLeft(3, '0')
                    ),
                    new HouseNumberWithBoxNumber(
                        HouseNumberSourceValue,
                        int.Parse(IndexSourceValue[1..]).ToString()
                    ),
                    new HouseNumberWithBoxNumber(
                        HouseNumberSourceValue,
                        TrimmedIndexSourceValue
                    )
                ];
            }

            if (!IndexSourceValue.StartsWith("b", StringComparison.InvariantCultureIgnoreCase) && char.ToLower(IndexSourceValue[1]) == 'b' && IsNumeric(IndexSourceValue[2..]))
            {
                var bisNumber = IndexSourceValue[0];
                var houseNumber = char.IsDigit(bisNumber) ? $"{HouseNumberSourceValue}_{bisNumber}" : $"{HouseNumberSourceValue}{bisNumber}";

                return
                [
                    new HouseNumberWithBoxNumber(
                        houseNumber,
                        int.Parse(IndexSourceValue[2..]).ToString().PadLeft(3, '0')
                    ),
                    new HouseNumberWithBoxNumber(
                        houseNumber,
                        int.Parse(IndexSourceValue[2..]).ToString().PadLeft(2, '0')
                    )
                ];
            }

            if (IndexSourceValue.StartsWith("000") && char.IsDigit(IndexSourceValue[3]))
            {
                var bisNumber = IndexSourceValue[3];
                var houseNumber = $"{HouseNumberSourceValue}_{bisNumber}";

                return
                [
                    new HouseNumberWithBoxNumber(
                        houseNumber,
                        null
                    )
                ];
            }

            if (char.IsLetter(IndexSourceValue[0]) && IsNumeric(IndexSourceValue[1..]))
            {
                return
                [
                    new HouseNumberWithBoxNumber(
                        HouseNumberSourceValue,
                        $"{IndexSourceValue[0]}{int.Parse(IndexSourceValue[1..])}"
                    )
                ];
            }

            if (IndexSourceValue.StartsWith("Glv", StringComparison.InvariantCultureIgnoreCase))
            {
                return
                [
                    new HouseNumberWithBoxNumber(
                        HouseNumberSourceValue,
                        "0.0"
                    )
                ];
            }

            throw new InvalidOperationException("Invalid use of matches");
        }
    }
}
