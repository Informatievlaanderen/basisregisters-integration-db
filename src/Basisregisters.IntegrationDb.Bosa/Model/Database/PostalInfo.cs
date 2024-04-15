namespace Basisregisters.IntegrationDb.Bosa.Model.Database
{
    using System;

    public record PostalInfo(string Namespace, string PostalCode, DateTimeOffset VersionTimestamp, string DutchName, string? CrabVersionTimestamp);
}
