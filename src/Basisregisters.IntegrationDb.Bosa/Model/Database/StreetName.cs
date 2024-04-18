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
        public string StreetNamePersistentLocalId { get; set; }
        public string Namespace { get; set; }

        public StreetNameStatus Status { get; set; }

        public DateTimeOffset VersionTimestamp { get; init; }
        public string? CrabVersionTimestamp { get; init; }

        public DateTimeOffset CreatedOn { get; set; }
        public string? NameDutch { get; }
        public string? NameFrench { get; }
        public string? NameGerman { get; }

        public string NisCode { get; set; }
        public string MunicipalityNamespace { get; set; }
        public DateTimeOffset MunicipalityVersionTimestamp { get; set; }
        public string? MunicipalityCrabVersionTimestamp { get; set; }

        // Needed for Dapper
        protected StreetName() { }

        public StreetName(
            string streetNamePersistentLocalId,
            string @namespace,
            StreetNameStatus status,
            DateTimeOffset versionTimestamp,
            string crabVersionTimestamp,
            DateTimeOffset createdOn,
            string? nameDutch,
            string? nameFrench,
            string? nameGerman,
            string nisCode,
            string municipalityNamespace,
            DateTimeOffset municipalityVersionTimestamp,
            string municipalityCrabVersionTimestamp)
        {
            StreetNamePersistentLocalId = streetNamePersistentLocalId;
            Namespace = @namespace;
            Status = status;
            VersionTimestamp = versionTimestamp;
            CrabVersionTimestamp = crabVersionTimestamp;
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
