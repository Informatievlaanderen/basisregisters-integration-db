namespace Basisregisters.IntegrationDb.Bosa.Model
{
    using System;

    public interface IHasVersionTimestamps
    {
        public DateTimeOffset VersionTimestamp { get; init; }
        public string? CrabVersionTimestamp { get; init; }
    }
}
