namespace Basisregisters.IntegrationDb.NationalRegistry.Tests.StreetNameMatching
{
    using System.Collections.Generic;
    using System.Linq;
    using AutoFixture;
    using FluentAssertions;
    using NationalRegistry.StreetNameMatching;
    using Repositories;
    using Xunit;

    public class LevenshteinDistanceMatchingTests
    {
        private readonly Fixture _fixture;
        private readonly IEnumerable<StreetName> _streetNames;

        public LevenshteinDistanceMatchingTests()
        {
            _fixture = new Fixture();
            _streetNames = _fixture.Create<IEnumerable<StreetName>>();
        }

        [Theory]
        [InlineData("Karkstraat")]
        [InlineData("Kérkstraat")]
        [InlineData("kerkstraat")]
        [InlineData("Kôrkstraat")]
        [InlineData("Kerkstrâät")]
        public void GivenDistanceIsLessThanOrEqualToMaxDistance_ThenMatchIsFound(string search)
        {
            var expectedStreetName = new StreetName(
                _fixture.Create<int>(),
                "Kerkstraat",
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

            var matched = matcher.MatchStreetName(_fixture.Create<string>(), search).FirstOrDefault();

            matched.Should().BeEquivalentTo(expectedStreetName);
        }

        [Fact]
        public void GivenDistanceIsMoreThanMaxDistance_ThenNoMatchIsFound()
        {
            var expectedStreetName = new StreetName(
                _fixture.Create<int>(),
                "Kerkstraat",
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

            var matched = matcher.MatchStreetName(_fixture.Create<string>(), "Kerktorenstraat").FirstOrDefault();

            matched.Should().BeNull();
        }

        [Theory]
        [InlineData("DR. DECROLYSTRAAT","Doctor Decrolystraat")]
        [InlineData("VAN STRIJDONCKLAAN","Van Strydoncklaan")]
        [InlineData("ZUSTERS VAN O.-L.-VROUWSTRAAT","Zusters van Onze-Lieve-Vrouwstraat")]
        public void Given(string search, string expected)
        {
            var expectedStreetName = new StreetName(
                _fixture.Create<int>(),
                expected,
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

            var matched = matcher.MatchStreetName(_fixture.Create<string>(), search).FirstOrDefault();

            matched.Should().BeEquivalentTo(expectedStreetName);
        }
    }
}
