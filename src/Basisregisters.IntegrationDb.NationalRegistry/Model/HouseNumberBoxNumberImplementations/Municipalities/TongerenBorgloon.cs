namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class TongerenBorgloon : MunicipalityHouseNumberBoxNumbersBase
    {
        public TongerenBorgloon(string nisCode, string sourceHouseNumber, NationalRegistryIndex index) : base(nisCode, sourceHouseNumber, index)
        { }

        public override bool IsMatch() =>
            NisCode == "73111" &&
            (
                IndexSourceValue.StartsWith("00b") || IndexSourceValue.StartsWith("0b")
            );

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            if (IndexSourceValue.StartsWith("00b") || IndexSourceValue.StartsWith("0b"))
            {
                var busNumber = IndexSourceValue.Split('b').Last();

                return
                [
                    new HouseNumberWithBoxNumber(
                        HouseNumberSourceValue,
                        busNumber
                    )
                ];
            }

            throw new InvalidOperationException("Invalid use of matches");
        }
    }
}
