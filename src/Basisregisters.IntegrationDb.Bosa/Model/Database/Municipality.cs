namespace Basisregisters.IntegrationDb.Bosa.Model.Database
{
    using System;

    public class Municipality
    {
        public string Namespace { get; init; } = null!;
        public string NisCode { get; init; } = null!;
        public DateTimeOffset VersionTimestamp { get; init; }
        public string DutchName { get; init; } = null!;
        public string? FrenchName { get; init; }
        public string? GermanName { get; init; }
        public string? EnglishName { get; init; }
        public MunicipalityStatus Status { get; init; }

        // Needed for dapper
        protected Municipality()
        { }

        public Municipality(
            string @namespace,
            string nisCode,
            DateTimeOffset versionTimestamp,
            string dutchName,
            string? frenchName,
            string? germanName,
            string? englishName,
            MunicipalityStatus status)
        {
            Namespace = @namespace;
            NisCode = nisCode;
            VersionTimestamp = versionTimestamp;
            DutchName = dutchName;
            FrenchName = frenchName;
            GermanName = germanName;
            EnglishName = englishName;
            Status = status;
        }
    }

    public enum MunicipalityStatus
    {
        Current = 0,
        Retired = 1,
        Proposed = 2,
    }
}
