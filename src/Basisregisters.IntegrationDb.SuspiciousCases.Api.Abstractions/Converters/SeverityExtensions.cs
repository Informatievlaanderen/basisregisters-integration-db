namespace Basisregisters.IntegrationDb.SuspiciousCases.Api.Abstractions.Converters
{
    using System;
    using Basisregisters.IntegrationDb.SuspiciousCases.Api.Abstractions;
    using Basisregisters.IntegrationDb.SuspiciousCases.Api.Abstractions.List;

    public static class SeverityExtensions
    {
        public static Ernst Map(this Severity severity)
        {
            return severity switch
            {
                Severity.Incorrect => Ernst.Foutief,
                Severity.Suspicious => Ernst.Verdacht,
                Severity.Improvable => Ernst.Verbeterbaar,
                _ => throw new ArgumentOutOfRangeException(nameof(severity), severity, null)
            };
        }
    }
}
