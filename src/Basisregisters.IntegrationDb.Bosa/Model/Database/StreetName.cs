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
        public string Namespace { get; init; } = null!;
        public int StreetNamePersistentLocalId { get; init; }

        public StreetNameStatus Status { get; init; }

        public DateTimeOffset VersionTimestamp { get; init; }

        public DateTimeOffset CreatedOn { get; init; }
        public string? NameDutch { get; init; }
        public string? NameFrench { get; init; }
        public string? NameGerman { get; init; }
        public string? HomonymAdditionDutch { get; init; }
        public string? HomonymAdditionFrench { get; init; }
        public string? HomonymAdditionGerman { get; init; }

        public string NisCode { get; init; } = null!;
        public string MunicipalityNamespace { get; init; } = null!;
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
            string? homonymAdditionDutch,
            string? homonymAdditionFrench,
            string? homonymAdditionGerman,
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
            HomonymAdditionDutch = homonymAdditionDutch;
            HomonymAdditionFrench = homonymAdditionFrench;
            HomonymAdditionGerman = homonymAdditionGerman;
            NisCode = nisCode;
            MunicipalityNamespace = municipalityNamespace;
            MunicipalityVersionTimestamp = municipalityVersionTimestamp;
            MunicipalityStatus = municipalityStatus;
        }
    }
}
