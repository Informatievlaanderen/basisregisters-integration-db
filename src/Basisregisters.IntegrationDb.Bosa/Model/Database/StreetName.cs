namespace Basisregisters.IntegrationDb.Bosa.Model.Database
{
    using System;

    public enum StreetNameStatus
    {
        Proposed = 0,
        Current = 1,
        Retired = 2,
        Rejected = 3
    }

    public class StreetName : IHasVersionTimestamps
    {
        public string StreetNamePersistentLocalId { get; init; }
        public string Namespace { get; init; }

        public StreetNameStatus Status { get; init; }

        public DateTimeOffset VersionTimestamp { get; init; }
        public string? CrabVersionTimestamp { get; init; }
        public string? CrabCreatedOn { get; init; }

        public DateTimeOffset CreatedOn { get; init; }
        public string? NameDutch { get; init; }
        public string? NameFrench { get; init; }
        public string? NameGerman { get; init; }

        public string NisCode { get; init; }
        public string MunicipalityNamespace { get; init; }
        public DateTimeOffset MunicipalityVersionTimestamp { get; init; }
        public string? MunicipalityCrabVersionTimestamp { get; init; }

        // Needed for Dapper
        protected StreetName()
        { }

        public StreetName(
            string @namespace,
            string streetNamePersistentLocalId,
            StreetNameStatus status,
            DateTimeOffset versionTimestamp,
            string? crabVersionTimestamp,
            string? crabCreatedOn,
            DateTimeOffset createdOn,
            string? nameDutch,
            string? nameFrench,
            string? nameGerman,
            string nisCode,
            string municipalityNamespace,
            DateTimeOffset municipalityVersionTimestamp,
            string municipalityCrabVersionTimestamp)
        {
            Namespace = @namespace;
            StreetNamePersistentLocalId = streetNamePersistentLocalId;
            Status = status;
            VersionTimestamp = versionTimestamp;
            CrabVersionTimestamp = crabVersionTimestamp;
            CrabCreatedOn = crabCreatedOn;
            CreatedOn = createdOn;
            NameDutch = nameDutch;
            NameFrench = nameFrench;
            NameGerman = nameGerman;
            NisCode = nisCode;
            MunicipalityNamespace = municipalityNamespace;
            MunicipalityVersionTimestamp = municipalityVersionTimestamp;
            MunicipalityCrabVersionTimestamp = municipalityCrabVersionTimestamp;
        }
    }
}
