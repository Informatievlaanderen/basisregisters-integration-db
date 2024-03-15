namespace Basisregisters.IntegrationDb.NationalRegistry.StreetNameMatching.Sanitizers
{
    using System;
    using System.Text.RegularExpressions;

    public class TrimEndSanitizer : SanitizerBase
    {
        private readonly (string pattern, string toTrim)[] _patterns =
        {
            ("RES.[a-zA-Z ]+", "RES.[a-zA-Z ]+"),
            ("[a-zA-Z ]+CENTRUM", "CENTRUM"),
            (@".*\(NIEUWE STRAAT\)$", @"\(NIEUWE STRAAT\)")
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
                if (Regex.IsMatch(sanitized, pattern.pattern))
                {
                    sanitized = Regex.Replace(sanitized, pattern.toTrim, "").Trim();
                }
            }

            return sanitized;
        }
    }
}
