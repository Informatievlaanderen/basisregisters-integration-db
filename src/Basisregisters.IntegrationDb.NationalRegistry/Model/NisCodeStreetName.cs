namespace Basisregisters.IntegrationDb.NationalRegistry.Model
{
    using FlatFiles.TypeMapping;

    public sealed class NisCodeStreetNameRecord
    {
        public string NisCode { get; set; }
        public string StreetName { get; set; }

        public static IDelimitedTypeMapper<NisCodeStreetNameRecord> Mapper
        {
            get
            {
                var mapper = DelimitedTypeMapper.Define<NisCodeStreetNameRecord>();
                mapper.Property(x => x.NisCode);
                mapper.Property(x => x.StreetName);
                return mapper;
            }
        }
    }
}
