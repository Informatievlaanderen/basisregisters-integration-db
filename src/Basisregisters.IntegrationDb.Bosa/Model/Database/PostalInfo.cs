namespace Basisregisters.IntegrationDb.Bosa.Model.Database
{
    using System;

    public class PostalInfo
    {
        public string Namespace { get; init; } = null!;
        public string PostalCode { get; init; } = null!;
        public DateTimeOffset VersionTimestamp { get; init; }
        public string DutchName { get; init; } = null!;

        // Needed for dapper
        protected PostalInfo() { }

        public PostalInfo(
            string @namespace,
            string postalCode,
            DateTimeOffset versionTimestamp,
            string dutchName)
        {
            Namespace = @namespace;
            PostalCode = postalCode;
            VersionTimestamp = versionTimestamp;
            DutchName = dutchName;
        }
    }
}
