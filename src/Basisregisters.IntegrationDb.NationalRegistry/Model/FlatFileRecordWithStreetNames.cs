namespace Basisregisters.IntegrationDb.NationalRegistry.Model
{
    using System.Collections.Generic;
    using Repositories;

    public class FlatFileRecordWithStreetNames
    {
        public FlatFileRecord Record { get; }
        public IList<StreetName> StreetNames { get; }

        public FlatFileRecordWithStreetNames(FlatFileRecord flatFileRecord, IList<StreetName> streetNames)
        {
            Record = flatFileRecord;
            StreetNames = streetNames;
        }
    }
}
