namespace Basisregisters.IntegrationDb.NationalRegistry.Model
{
    using System.Collections.Generic;
    using Repositories;

    public class FlatFileRecordWithStreetNames
    {
        public FlatFileRecord Record { get; }
        public List<StreetName> StreetNames { get; }

        public FlatFileRecordWithStreetNames(FlatFileRecord flatFileRecord, List<StreetName> streetNames)
        {
            Record = flatFileRecord;
            StreetNames = streetNames;
        }
    }
}
