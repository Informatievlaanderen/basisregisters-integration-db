namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Boom : MunicipalityHouseNumberBoxNumbersBase
    {
        public Boom(string nisCode, string sourceHouseNumber, NationalRegistryIndex index) : base(nisCode, sourceHouseNumber, index)
        { }

        public override bool IsMatch() =>
            NisCode == "11005" &&
            (
                IndexSourceValue.Contains('.')
            );

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            if (IndexSourceValue.Contains('.'))
            {
                var indexWithoutLetters = string.Join("", IndexSourceValue.Where(x => !IsLetter(x)));

                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        HouseNumberSourceValue,
                        indexWithoutLetters.Trim()
                    )
                };
            }

            throw new InvalidOperationException("Invalid use of matches");
        }
    }
}
