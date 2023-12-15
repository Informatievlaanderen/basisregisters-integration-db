namespace Basisregisters.IntegrationDb.SuspiciousCases.Api.Converters
{
    using System;
    using List;

    public static class SeverityExtensions
    {
        public static Ernst Map(this Severity severity)
        {
            return severity switch
            {
                Severity.Incorrect => Ernst.Foutief,
                Severity.Suspicious => Ernst.Verdacht,
                Severity.Improveable => Ernst.Verbeterbaar,
                _ => throw new ArgumentOutOfRangeException(nameof(severity), severity, null)
            };
        }
    }
}
