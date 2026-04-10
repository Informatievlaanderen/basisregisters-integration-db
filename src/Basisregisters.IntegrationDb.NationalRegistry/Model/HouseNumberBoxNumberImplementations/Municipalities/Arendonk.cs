namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    using System;
    using System.Collections.Generic;

    public class Arendonk : MunicipalityHouseNumberBoxNumbersBase
    {
        public Arendonk(string nisCode, string sourceHouseNumber, NationalRegistryIndex index) : base(nisCode, sourceHouseNumber, index)
        { }

        public override bool IsMatch() =>
            NisCode == "13001" &&
            (
                (IndexSourceValue.Length == 4 && IsLetter(IndexSourceValue[0]) && IndexSourceValue[1..] == "000")
                ||
                (IndexSourceValue.Length == 4 && IsLetter(IndexSourceValue[0]) && IndexSourceValue[1] == 'b' && IsNumeric(IndexSourceValue[2..]))
            );

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            if (IndexSourceValue.Length == 4 && IsLetter(IndexSourceValue[0]) && IndexSourceValue[1..] == "000")
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        HouseNumberSourceValue,
                        IndexSourceValue[0].ToString()
                    )
                };
            }

            if (IndexSourceValue.Length == 4 && IsLetter(IndexSourceValue[0]) && IndexSourceValue[1] == 'b' && IsNumeric(IndexSourceValue[2..]))
            {
                if(IndexSourceValue[0] == 'B')
                {
                    return new[]
                    {
                        new HouseNumberWithBoxNumber(
                            HouseNumberSourceValue,
                            IndexSourceValue
                        )
                    };
                }
                else
                {
                    return new[]
                    {
                        new HouseNumberWithBoxNumber(
                            HouseNumberSourceValue + IndexSourceValue[0],
                            IndexSourceValue[1..]
                        ),
                        new HouseNumberWithBoxNumber(
                            HouseNumberSourceValue + IndexSourceValue[0],
                            IndexSourceValue[2..].TrimStart('0')
                        )
                    };
                }
            }

            throw new InvalidOperationException("Invalid use of matches");
        }
    }
}
