namespace Basisregisters.IntegrationDb.NationalRegistry.StreetNameMatching.Sanitizers
{
    using System;
    using System.Text.RegularExpressions;

    public class UseRoundBracketsSuffixAsPrefixSanitizer : SanitizerBase
    {
        public UseRoundBracketsSuffixAsPrefixSanitizer() : this(Array.Empty<SanitizerBase>())
        { }

        public UseRoundBracketsSuffixAsPrefixSanitizer(SanitizerBase[] sanitizers) : base(sanitizers)
        { }

        protected override string InnerSanitize(string streetName)
        {
            const string pattern = @"\(([^)]*)\)$";

            var match = Regex.Match(streetName, pattern);

            if (!match.Success)
            {
                return streetName;
            }

            var suffix = match.Groups[1].Value;

            var result = Regex.Replace(streetName, pattern, "");

            result = suffix + " " + result.Trim();

            return result;
        }
    }
}
