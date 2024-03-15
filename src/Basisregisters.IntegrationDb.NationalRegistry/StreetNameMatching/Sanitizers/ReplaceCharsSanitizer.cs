namespace Basisregisters.IntegrationDb.NationalRegistry.StreetNameMatching.Sanitizers
{
    using System;

    public class ReplaceCharsSanitizer : SanitizerBase
    {
        public ReplaceCharsSanitizer() : this(Array.Empty<SanitizerBase>())
        { }

        public ReplaceCharsSanitizer(SanitizerBase[] sanitizers) : base(sanitizers)
        { }

        private readonly (string oldValue, string newValue)[] _charsToReplace =
        {
            ("ij", "y"), // VAN STRIJDONCKLAAN -> Van Strydoncklaan
            ("ii", "11"), // II JULILAAN -> 11 Julilaan
            ("-", " "), // BOS-VAN-STEENSTRAAT -> Bos van Steenstraat
            ("\"", "") // WIJKDE STAD-> Wijk"De Stad"
        };

        protected override string InnerSanitize(string streetName)
        {
            var sanitized = streetName;

            foreach (var replacement in _charsToReplace)
            {
                sanitized = sanitized.Replace(replacement.oldValue, replacement.newValue, StringComparison.InvariantCultureIgnoreCase);
            }

            return sanitized;
        }
    }
}
