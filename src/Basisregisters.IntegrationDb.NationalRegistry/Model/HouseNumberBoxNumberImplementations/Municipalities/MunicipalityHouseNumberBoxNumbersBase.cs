namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    public abstract class MunicipalityHouseNumberBoxNumbersBase : HouseNumberBoxNumbersBase
    {
        public string IndexSourceValue { get; }
        public string TrimmedIndexSourceValue { get; }

        protected MunicipalityHouseNumberBoxNumbersBase(string nisCode, string sourceHouseNumber, NationalRegistryIndex index)
            : base(nisCode, sourceHouseNumber, index)
        {
            IndexSourceValue = (Index.SourceValue ?? string.Empty).PadRight(4, ' ');
            TrimmedIndexSourceValue = (Index.SourceValue ?? string.Empty).Trim();
        }
    }
}
