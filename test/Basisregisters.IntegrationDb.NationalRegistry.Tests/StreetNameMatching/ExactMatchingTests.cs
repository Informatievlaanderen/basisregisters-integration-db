﻿namespace Basisregisters.IntegrationDb.NationalRegistry.Tests.StreetNameMatching
{
    using System.Collections.Generic;
    using System.Linq;
    using AutoFixture;
    using FluentAssertions;
    using Matching;
    using Repositories;
    using Xunit;

    public class ExactMatchingTests
    {
        private readonly Fixture _fixture;
        private readonly IEnumerable<StreetName> _streetNames;

        public ExactMatchingTests()
        {
            _fixture = new Fixture();
            _streetNames = _fixture.Create<IEnumerable<StreetName>>();
        }

        [Theory]
        [InlineData("kerkstraat", "kerkstraat")]
        [InlineData("Kerkstraat", "Kerkstraat")]
        [InlineData("DeStRaat", "dEsTraAt")]
        public void DutchStreetName(string streetName, string search)
        {
            var expectedStreetName = new StreetName(
                _fixture.Create<int>(),
                streetName,
                null,
                null,
                null,
                null,
                null,
                null,
                null);
            var streetNames = _streetNames.Concat(new[] { expectedStreetName });
            var matcher = new StreetNameMatcher(streetNames);

            var persistentLocalId = matcher.MatchStreetName(search);

            persistentLocalId.Should().Be(expectedStreetName.StreetNamePersistentLocalId);
        }

        [Theory]
        [InlineData("kerkstraat", "kerkstraat")]
        [InlineData("Kerkstraat", "Kerkstraat")]
        [InlineData("DeStRaat", "dEsTraAt")]
        public void FrenchStreetName(string streetName, string search)
        {
            var expectedStreetName = new StreetName(
                _fixture.Create<int>(),
                null,
                streetName,
                null,
                null,
                null,
                null,
                null,
                null);
            var streetNames = _streetNames.Concat(new[] { expectedStreetName });
            var matcher = new StreetNameMatcher(streetNames);

            var persistentLocalId = matcher.MatchStreetName(search);

            persistentLocalId.Should().Be(expectedStreetName.StreetNamePersistentLocalId);
        }

        [Theory]
        [InlineData("kerkstraat", "kerkstraat")]
        [InlineData("Kerkstraat", "Kerkstraat")]
        [InlineData("DeStRaat", "dEsTraAt")]
        public void GermanStreetName(string streetName, string search)
        {
            var expectedStreetName = new StreetName(
                _fixture.Create<int>(),
                null,
                null,
                streetName,
                null,
                null,
                null,
                null,
                null);
            var streetNames = _streetNames.Concat(new[] { expectedStreetName });
            var matcher = new StreetNameMatcher(streetNames);

            var persistentLocalId = matcher.MatchStreetName(search);

            persistentLocalId.Should().Be(expectedStreetName.StreetNamePersistentLocalId);
        }

        [Theory]
        [InlineData("kerkstraat", "kerkstraat")]
        [InlineData("Kerkstraat", "Kerkstraat")]
        [InlineData("DeStRaat", "dEsTraAt")]
        public void EnglishStreetName(string streetName, string search)
        {
            var expectedStreetName = new StreetName(
                _fixture.Create<int>(),
                null,
                null,
                null,
                streetName,
                null,
                null,
                null,
                null);
            var streetNames = _streetNames.Concat(new[] { expectedStreetName });
            var matcher = new StreetNameMatcher(streetNames);

            var persistentLocalId = matcher.MatchStreetName(search);

            persistentLocalId.Should().Be(expectedStreetName.StreetNamePersistentLocalId);
        }
    }
}
