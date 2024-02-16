namespace Basisregisters.IntegrationDb.NationalRegistry.Tests.WhenMatchingStreetNames
{
    using System.Collections.Generic;
    using System.Linq;
    using AutoFixture;
    using FluentAssertions;
    using Matching;
    using Repositories;
    using Xunit;

    public class LevenshteinDistanceMatchingTests
    {
        private readonly Fixture _fixture;
        private readonly IEnumerable<StreetName> _streetNames;

        private const int MaxLevenshteinDistanceInPercentage = 10;

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
                null);
            var streetNames = _streetNames.Concat(new[] { expectedStreetName });
            var matcher = new StreetNameMatcher(streetNames, MaxLevenshteinDistanceInPercentage);

            var persistentLocalId = matcher.MatchStreetName(search);

            persistentLocalId.Should().Be(expectedStreetName.StreetNamePersistentLocalId);
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
                null);
            var streetNames = _streetNames.Concat(new[] { expectedStreetName });
            var matcher = new StreetNameMatcher(streetNames, MaxLevenshteinDistanceInPercentage);

            var persistentLocalId = matcher.MatchStreetName("Kerkenstraat");

            persistentLocalId.Should().BeNull();
        }
    }
}
