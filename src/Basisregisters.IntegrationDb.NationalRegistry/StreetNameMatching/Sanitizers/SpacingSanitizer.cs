namespace Basisregisters.IntegrationDb.NationalRegistry.StreetNameMatching.Sanitizers
{
    using System;
    using System.Text.RegularExpressions;

    public class SpacingSanitizer : SanitizerBase
    {
        public SpacingSanitizer() : this(Array.Empty<SanitizerBase>())
        { }

        public SpacingSanitizer(SanitizerBase[] sanitizers) : base(sanitizers)
        { }

        protected override string InnerSanitize(string streetName)
        {
            // BURG.H.DE BACKERESTRAAT -> Burgemeester H. De Backerestraat
            const string pattern = @"\.(?=[^\s])";

            var replacement = ". ";

            return Regex.Replace(streetName, pattern, replacement);
        }
    }
}
