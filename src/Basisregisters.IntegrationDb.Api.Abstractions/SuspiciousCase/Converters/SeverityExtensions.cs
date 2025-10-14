namespace Basisregisters.IntegrationDb.Api.Abstractions.SuspiciousCase.Converters
{
    using System;
    using Basisregisters.IntegrationDb.SuspiciousCases;
    using List;

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
