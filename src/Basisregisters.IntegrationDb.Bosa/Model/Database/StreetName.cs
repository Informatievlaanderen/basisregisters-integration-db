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

    public class StreetName
    {
        public string Namespace { get; init; }
        public int StreetNamePersistentLocalId { get; init; }

        public StreetNameStatus Status { get; init; }

        public DateTimeOffset VersionTimestamp { get; init; }

        public DateTimeOffset CreatedOn { get; init; }
        public string? NameDutch { get; init; }
        public string? NameFrench { get; init; }
        public string? NameGerman { get; init; }

        public string NisCode { get; init; }
        public string MunicipalityNamespace { get; init; }
        public DateTimeOffset MunicipalityVersionTimestamp { get; init; }
        public MunicipalityStatus MunicipalityStatus { get; init; }

        // Needed for Dapper
        protected StreetName()
        { }

        public StreetName(
            string @namespace,
            int streetNamePersistentLocalId,
            StreetNameStatus status,
            DateTimeOffset versionTimestamp,
            DateTimeOffset createdOn,
            string? nameDutch,
            string? nameFrench,
            string? nameGerman,
            string nisCode,
            string municipalityNamespace,
            DateTimeOffset municipalityVersionTimestamp,
            MunicipalityStatus municipalityStatus)
        {
            Namespace = @namespace;
            StreetNamePersistentLocalId = streetNamePersistentLocalId;
            Status = status;
            VersionTimestamp = versionTimestamp;
            CreatedOn = createdOn;
            NameDutch = nameDutch;
            NameFrench = nameFrench;
            NameGerman = nameGerman;
            NisCode = nisCode;
            MunicipalityNamespace = municipalityNamespace;
            MunicipalityVersionTimestamp = municipalityVersionTimestamp;
            MunicipalityStatus = municipalityStatus;
        }
    }
}
