namespace Basisregisters.IntegrationDb.Bosa.Model.Database
{
    using System;

    public class PostalInfo
    {
        public string Namespace { get; init; }
        public string PostalCode { get; init; }
        public DateTimeOffset VersionTimestamp { get; init; }
        public string DutchName { get; init; }

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
