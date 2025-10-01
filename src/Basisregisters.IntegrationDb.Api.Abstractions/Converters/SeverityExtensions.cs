namespace Basisregisters.IntegrationDb.Api.Abstractions.Converters
{
    using System;
    using List;
    using SuspiciousCases;

    public static class SeverityExtensions
    {
        public static Ernst Map(this Severity severity)
        {
            return severity switch
            {
                Severity.Incorrect => Ernst.Foutief,
                Severity.Suspicious => Ernst.Verdacht,
                _ => throw new ArgumentOutOfRangeException(nameof(severity), severity, null)
            };
        }
    }
}
