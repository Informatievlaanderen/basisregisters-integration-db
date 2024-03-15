namespace Basisregisters.IntegrationDb.NationalRegistry.Tests.StreetNameMatching
{
    using System.Collections.Generic;
    using System.Linq;
    using AutoFixture;
    using FluentAssertions;
    using NationalRegistry.StreetNameMatching;
    using Repositories;
    using Xunit;

    public class AbbreviationMatchingTests
    {
        private readonly Fixture _fixture;
        private readonly IEnumerable<StreetName> _streetNames;

        public AbbreviationMatchingTests()
        {
            _fixture = new Fixture();
            _streetNames = _fixture.Create<IEnumerable<StreetName>>();
        }

        [Theory]
        [InlineData("onze lieve vrouw", "O.L. Vrouw")]
        [InlineData("o.l.v.", "Onze Lieve Vrouw")]
        [InlineData("onze-lieve-", "O.L. ")]
        [InlineData("sint", "st.")]
        [InlineData("st.", "sint")]
        [InlineData("dr.", "dokter")]
        [InlineData("dokter", "dr.")]
        [InlineData("stwg.", "steenweg")]
        [InlineData("stwg", "steenweg")]
        [InlineData("stw.", "steenweg")]
        [InlineData("stw", "steenweg")]
        [InlineData("burg.", "Burgemeester")]
        [InlineData("Burgemeester", "burg.")]
        public void WithContainsAbbreviation_ThenMatchIsFound(string search, string substring)
        {
            var prefix = $"{_fixture.Create<string>()}";
            var suffix = $"{_fixture.Create<string>()}";

            var streetName = $"{prefix}{substring}{suffix}";
            var searchWithAbbrevation = $"{prefix}{search}{suffix}";

            var expectedStreetName = new StreetName(
                _fixture.Create<int>(),
                streetName,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                string.Empty,
                string.Empty);
            var streetNames = _streetNames.Concat(new[] { expectedStreetName });
            var matcher = new StreetNameMatcher(streetNames);

            var matched = matcher.MatchStreetName(_fixture.Create<string>(), searchWithAbbrevation).FirstOrDefault();

            matched.Should().BeEquivalentTo(expectedStreetName);
        }

        [Theory]
        [InlineData("str", "straat")]
        [InlineData("str.", "straat")]
        public void WithEndsWithAbbreviation_ThenMatchIsFound(string search, string substring)
        {
            var prefix = $"{_fixture.Create<string>()}";

            var streetName = $"{prefix}{substring}";
            var searchWithAbbrevation = $"{prefix}{search}";

            var expectedStreetName = new StreetName(
                _fixture.Create<int>(),
                streetName,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                string.Empty,
                string.Empty);
            var streetNames = _streetNames.Concat(new[] { expectedStreetName });
            var matcher = new StreetNameMatcher(streetNames);

            var matched = matcher.MatchStreetName(_fixture.Create<string>(), searchWithAbbrevation).FirstOrDefault();

            matched.Should().BeEquivalentTo(expectedStreetName);
        }

        [Theory]
        [InlineData("heilige", "h")]
        [InlineData("heilig", "h")]
        [InlineData("k.", "koning")]
        public void WithStartsWithAbbreviation_ThenMatchIsFound(string search, string substring)
        {
            var suffix = $"{_fixture.Create<string>()}";

            var streetName = $"{substring}{suffix}";
            var searchWithAbbrevation = $"{search}{suffix}";

            var expectedStreetName = new StreetName(
                _fixture.Create<int>(),
                streetName,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                string.Empty,
                string.Empty);
            var streetNames = _streetNames.Concat(new[] { expectedStreetName });
            var matcher = new StreetNameMatcher(streetNames);

            var matched = matcher.MatchStreetName(_fixture.Create<string>(), searchWithAbbrevation).FirstOrDefault();

            matched.Should().BeEquivalentTo(expectedStreetName);
        }
    }
}
