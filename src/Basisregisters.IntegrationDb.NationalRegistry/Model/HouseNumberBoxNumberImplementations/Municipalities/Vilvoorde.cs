namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    using System.Collections.Generic;

    public class Vilvoorde : MunicipalityHouseNumberBoxNumbersBase
    {
        public Vilvoorde(string nisCode, string sourceHouseNumber, NationalRegistryIndex index) : base(nisCode, sourceHouseNumber, index)
        { }

        public override bool IsMatch() => NisCode == "23088";

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            if (TrimmedIndexSourceValue.Length == 4
                && IsLetter(IndexSourceValue[0])
                && IndexSourceValue.IndexOf('/') == 2)
            {
                return new List<HouseNumberWithBoxNumber>
                {
                    new HouseNumberWithBoxNumber(
                        $"{HouseNumberSourceValue}{IndexSourceValue[0]}",
                        IndexSourceValue[1..])
                };
            }
            
            return new List<HouseNumberWithBoxNumber>
            {
                new HouseNumberWithBoxNumber(
                    HouseNumberSourceValue,
                    TrimmedIndexSourceValue)
            };
        }
    }
}
