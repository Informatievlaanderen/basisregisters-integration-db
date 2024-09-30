namespace Basisregisters.IntegrationDb.Bosa.Model.Database
{
    using System;

    public class Municipality : IHasVersionTimestamps
    {
        public string Namespace { get; init; }
        public string NisCode { get; init; }
        public DateTimeOffset VersionTimestamp { get; init; }
        public string DutchName { get; init; }
        public string? FrenchName { get; init; }
        public string? GermanName { get; init; }
        public string? EnglishName { get; init; }
        public string? CrabVersionTimestamp { get; init; }
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
            string? crabVersionTimestamp,
            MunicipalityStatus status)
        {
            Namespace = @namespace;
            NisCode = nisCode;
            VersionTimestamp = versionTimestamp;
            DutchName = dutchName;
            FrenchName = frenchName;
            GermanName = germanName;
            EnglishName = englishName;
            CrabVersionTimestamp = crabVersionTimestamp;
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
