namespace Basisregisters.IntegrationDb.NationalRegistry.Model
{
    using FlatFiles;
    using FlatFiles.TypeMapping;

    public sealed class FlatFileRecord
    {
        public string NisCode { get; set; }
        public string PostalCode { get; set; }
        public string StreetCode { get; set; }
        public string HouseNumber { get; set; }
        public string Index { get; set; }
        public string StreetName { get; set; }
        public int RegisteredCount { get; set; }

        public int RecordNumber { get; set; }

        // public static FixedLengthSchema Schema
        // {
        //     get
        //     {
        //         var schema = new FixedLengthSchema();
        //         schema.AddColumn(new StringColumn("NisCode"), 5);
        //         schema.AddColumn(new StringColumn("PostalCode"), 4);
        //         schema.AddColumn(new StringColumn("StreetCode"), 4);
        //         schema.AddColumn(new StringColumn("HouseNumber"), 4);
        //         schema.AddColumn(new StringColumn("Index"), 4);
        //         schema.AddColumn(new StringColumn("StreetName"), 32);
        //         schema.AddColumn(new Int32Column("RegisteredCount"), 4);
        //         return schema;
        //     }
        // }

        public static IFixedLengthTypeMapper<FlatFileRecord> Mapper
        {
            get
            {
                var mapper = FixedLengthTypeMapper.Define<FlatFileRecord>();
                mapper.Property(x => x.NisCode, 5);
                mapper.Property(x => x.PostalCode, 4);
                mapper.Property(x => x.StreetCode, 4);
                mapper.Property(x => x.HouseNumber, 4);
                mapper.Property(x => x.Index, 4);
                mapper.Property(x => x.StreetName, 32);
                mapper.Property(x => x.RegisteredCount, 4);
                mapper
                    .CustomMapping(new RecordNumberColumn("RecordNumber"), Window.Trailing)
                    .WithReader(x => x.RecordNumber);
                return mapper;
            }
        }
    }
}
