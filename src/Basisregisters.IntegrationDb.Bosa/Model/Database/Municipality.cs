namespace Basisregisters.IntegrationDb.Bosa.Model.Database
{
    using System;

    public class Municipality : IHasVersionTimestamps
    {
        public string Namespace { get; init; }
        public string NisCode { get; init; }
        public DateTimeOffset VersionTimestamp { get; init; }
        public string DutchName { get; init; }
        public string? CrabVersionTimestamp { get; init; }

        // Needed for dapper
        protected Municipality() { }

        public Municipality(
            string @namespace,
            string nisCode,
            DateTimeOffset versionTimestamp,
            string dutchName,
            string? crabVersionTimestamp)
        {
            Namespace = @namespace;
            NisCode = nisCode;
            VersionTimestamp = versionTimestamp;
            DutchName = dutchName;
            CrabVersionTimestamp = crabVersionTimestamp;
        }
    }
}
