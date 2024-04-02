namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    using System.Collections.Generic;

    public class Vilvoorde : HouseNumberBoxNumbersBase
    {
        public Vilvoorde(string nisCode, string sourceHouseNumber, NationalRegistryIndex index) : base(nisCode, sourceHouseNumber, index)
        { }

        public override bool IsMatch() => NisCode == "23088";

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            if (char.IsLetter(Index.SourceValue![0]) && Index.SourceValue!.Contains("/"))
            {
                return new List<HouseNumberWithBoxNumber>
                {
                    new HouseNumberWithBoxNumber(
                        $"{SourceSourceHouseNumber}{Index.SourceValue![0]}",
                        Index.SourceValue.Substring(1))
                };
            }

            return new List<HouseNumberWithBoxNumber>
            {
                new HouseNumberWithBoxNumber(
                    SourceSourceHouseNumber,
                    Index.SourceValue)
            };
        }
    }
}
