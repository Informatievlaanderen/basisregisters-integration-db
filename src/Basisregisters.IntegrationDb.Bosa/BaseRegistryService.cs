namespace Basisregisters.IntegrationDb.Bosa
{
    using System;
    using Extensions;

    public class BaseRegistryService
    {
        protected static string GetVersionAsString(DateTimeOffset value) => value.ToBelgianString();
        protected static string FormatNamespace(string @namespace) => @namespace.EndsWith('/') ? @namespace : $"{@namespace}/";
    }
}
