namespace Basisregisters.IntegrationDb.NationalRegistry.StreetNameMatching.Sanitizers
{
    using System;
    using System.Text.RegularExpressions;

    public class RemoveRoundBracketsSuffixSanitizer : SanitizerBase
    {
        public RemoveRoundBracketsSuffixSanitizer() : this(Array.Empty<SanitizerBase>())
        { }

        public RemoveRoundBracketsSuffixSanitizer(SanitizerBase[] sanitizers) : base(sanitizers)
        { }

        protected override string InnerSanitize(string streetName)
        {
            const string closingBracketsPattern = @"\([^()]*\)$";

            var match = Regex.Match(streetName, closingBracketsPattern);

            if (match.Success)
            {
                return Regex.Replace(streetName, closingBracketsPattern, "").Trim();
            }

            const string openBracketsPattern = @"\([^()]*\)?$";

            return Regex.Replace(streetName, openBracketsPattern, "").Trim();
        }
    }
}
