namespace Basisregisters.IntegrationDb.NationalRegistry.Tests.StreetNameMatching.Sanitizers
{
    using AutoFixture;
    using Basisregisters.IntegrationDb.NationalRegistry.StreetNameMatching.Sanitizers;
    using FluentAssertions;
    using Xunit;

    public class TrimEndSanitizerTests
    {
        private readonly Fixture _fixture = new Fixture();

        [Theory]
        [InlineData("DONKWEG CENTRUM", "DONKWEG")]
        [InlineData("KERKSTRAAT RES. KONING ALBERT II", "KERKSTRAAT")]
        public void WhenStringToTrimIsAtTheEnd_ThenReturnsSanitizedValue(string value, string expected)
        {
            var sanitized = new TrimEndSanitizer(new SanitizerBase[]
                {
                    new AbbreviationSanitizer(),
                    new UseRoundBracketsSuffixAsPrefixSanitizer()
                })
                .Sanitize(_fixture.Create<string>(), value);

            sanitized.Search.Should().BeEquivalentTo(expected);
        }
    }
}
