namespace Basisregisters.IntegrationDb.Bosa
{
    using System;
    using Be.Vlaanderen.Basisregisters.GrAr.Extracts;
    using Extensions;

    public class BaseRegistryService
    {
        protected static string GetVersionAsString(DateTimeOffset value) => value.ToBelgianString();
        protected static string GetZuluVersionAsString(DateTimeOffset value) => value
            .ToUniversalTime()
            .ToString("yyyy-MM-ddTHH:mm:ssZ", System.Globalization.CultureInfo.InvariantCulture);

        protected static string FormatNamespace(string @namespace) => @namespace.EndsWith('/') ? @namespace : $"{@namespace}/";
    }
}
