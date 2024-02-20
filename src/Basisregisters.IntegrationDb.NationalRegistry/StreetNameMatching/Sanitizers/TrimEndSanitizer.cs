namespace Basisregisters.IntegrationDb.NationalRegistry.StreetNameMatching.Sanitizers
{
    using System;
    using System.Text.RegularExpressions;

    public class TrimEndSanitizer : SanitizerBase
    {
        private readonly string[] _patterns = new[]
        {
            "RES.[a-zA-Z ]+",
            "[a-zA-Z ]+CENTRUM",
            @".*\(NIEUWE STRAAT\)$"
        };

        public TrimEndSanitizer() : this(Array.Empty<SanitizerBase>())
        { }

        public TrimEndSanitizer(SanitizerBase[] sanitizers) : base(sanitizers)
        { }

        protected override string InnerSanitize(string streetName)
        {
            var sanitized = streetName;

            foreach (var pattern in _patterns)
            {
                sanitized = Regex.Replace(sanitized, pattern, "").Trim();
            }

            return sanitized;
        }
    }
}
