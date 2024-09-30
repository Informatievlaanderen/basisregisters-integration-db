namespace Basisregisters.IntegrationDb.Bosa.Model.Database
{
    using System;

    public class StreetNameIdentifier
    {
        public string Namespace { get; init; }
        public int StreetNamePersistentLocalId { get; init; }
        public DateTimeOffset VersionTimestamp { get; init; }
        public string? CrabVersionTimestamp { get; init; }
        public string NisCode { get; init; }
        public MunicipalityStatus MunicipalityStatus { get; init; }

        // Needed for Dapper
        protected StreetNameIdentifier()
        { }

        public StreetNameIdentifier(
            string @namespace,
            int streetNamePersistentLocalId,
            DateTimeOffset versionTimestamp,
            string? crabVersionTimestamp,
            string nisCode,
            MunicipalityStatus municipalityStatus)
        {
            Namespace = @namespace;
            StreetNamePersistentLocalId = streetNamePersistentLocalId;
            VersionTimestamp = versionTimestamp;
            CrabVersionTimestamp = crabVersionTimestamp;
            NisCode = nisCode;
            MunicipalityStatus = municipalityStatus;
        }
    }
}
