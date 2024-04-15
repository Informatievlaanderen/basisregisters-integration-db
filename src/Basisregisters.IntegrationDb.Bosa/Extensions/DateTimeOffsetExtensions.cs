namespace Basisregisters.IntegrationDb.Bosa.Extensions
{
    using System;
    using Be.Vlaanderen.Basisregisters.GrAr.Common;
    using Be.Vlaanderen.Basisregisters.GrAr.Extracts;
    using NodaTime;

    public static class DateTimeOffsetExtensions
    {
        public static string ToBelgianString(this DateTimeOffset dateTimeOffset)
        {
            return Instant
                .FromDateTimeOffset(dateTimeOffset)
                .ToBelgianDateTimeOffset()
                .FromDateTimeOffset();
        }
    }
}
