namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    public abstract class MunicipalityHouseNumberBoxNumbersBase : HouseNumberBoxNumbersBase
    {
        public string IndexSourceValue { get; }

        protected MunicipalityHouseNumberBoxNumbersBase(string nisCode, string sourceHouseNumber, NationalRegistryIndex index)
            : base(nisCode, sourceHouseNumber, index)
        {
            IndexSourceValue = Index.SourceValue!.PadRight(4, ' ');
        }
    }
}
