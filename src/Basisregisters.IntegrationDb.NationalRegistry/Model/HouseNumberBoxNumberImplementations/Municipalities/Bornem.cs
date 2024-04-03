namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    using System.Collections.Generic;

    public class Bornem : MunicipalityHouseNumberBoxNumbersBase
    {
        public Bornem(string nisCode, string sourceHouseNumber, NationalRegistryIndex index) : base(nisCode, sourceHouseNumber, index)
        { }

        public override bool IsMatch() => NisCode == "12007";

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            return new[]
            {
                new HouseNumberWithBoxNumber(
                    SourceSourceHouseNumber,
                    IndexSourceValue
                )
            };
        }
    }
}
