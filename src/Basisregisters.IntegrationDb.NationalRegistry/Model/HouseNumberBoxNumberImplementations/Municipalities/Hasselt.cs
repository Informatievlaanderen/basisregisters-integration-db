namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    public class Hasselt : MunicipalityHouseNumberBoxNumbersBase
    {
        public Hasselt(string nisCode, string sourceHouseNumber, NationalRegistryIndex index) : base(nisCode, sourceHouseNumber, index)
        { }

        public override bool IsMatch() => NisCode == "71072" && (
            (IndexSourceValue.Length == 4 && Regex.IsMatch(IndexSourceValue, "^[a-zA-Z]{1}[0-9]{1}.[0-9]{1}$"))
        );

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            if (IndexSourceValue.Length == 4 && Regex.IsMatch(IndexSourceValue, "^[a-zA-Z]{1}[0-9]{1}.[0-9]{1}$"))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        $"{HouseNumberSourceValue}{IndexSourceValue[0]}",
                        $"{IndexSourceValue[1]}.0{IndexSourceValue[3]}"
                    ),
                    new HouseNumberWithBoxNumber(
                        $"{HouseNumberSourceValue}{IndexSourceValue[0]}",
                        $"{IndexSourceValue[1]}.{IndexSourceValue[3]}"
                    )
                };
            }

            throw new InvalidOperationException("Invalid use of matches");
        }
    }
}
