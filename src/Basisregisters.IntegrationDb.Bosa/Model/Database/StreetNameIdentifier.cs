namespace Basisregisters.IntegrationDb.Bosa.Model.Database
{
    using System;

    public class StreetNameIdentifier
    {
        public string StreetNamePersistentLocalId { get; init; }
        public string Namespace { get; init; }
        public DateTimeOffset VersionTimestamp { get; init; }
        public string? CrabVersionTimestamp { get; init; }

        public StreetNameIdentifier(
            string streetNamePersistentLocalId,
            string @namespace,
            DateTimeOffset versionTimestamp,
            string? crabVersionTimestamp)
        {
            StreetNamePersistentLocalId = streetNamePersistentLocalId;
            Namespace = @namespace;
            VersionTimestamp = versionTimestamp;
            CrabVersionTimestamp = crabVersionTimestamp;
        }


        // Needed for Dapper
        protected StreetNameIdentifier()
        { }
    }
}
