namespace Basisregisters.IntegrationDb.Bosa.Model.Database
{
    using System;

    public class StreetNameIdentifier
    {
        public string Namespace { get; init; } = null!;
        public int StreetNamePersistentLocalId { get; init; }
        public DateTimeOffset VersionTimestamp { get; init; }
        public string NisCode { get; init; } = null!;
        public MunicipalityStatus MunicipalityStatus { get; init; }

        // Needed for Dapper
        protected StreetNameIdentifier()
        { }

        public StreetNameIdentifier(
            string @namespace,
            int streetNamePersistentLocalId,
            DateTimeOffset versionTimestamp,
            string nisCode,
            MunicipalityStatus municipalityStatus)
        {
            Namespace = @namespace;
            StreetNamePersistentLocalId = streetNamePersistentLocalId;
            VersionTimestamp = versionTimestamp;
            NisCode = nisCode;
            MunicipalityStatus = municipalityStatus;
        }
    }
}
