namespace Basisregisters.IntegrationDb.NationalRegistry.Tests.StreetNameMatching
{
    using System.Collections.Generic;
    using System.Linq;
    using AutoFixture;
    using FluentAssertions;
    using NationalRegistry.StreetNameMatching;
    using Repositories;
    using Xunit;

    public class ExactMatchingTests
    {
        private readonly Fixture _fixture;
        private readonly IEnumerable<StreetName> _streetNames;

        public ExactMatchingTests()
        {
            _fixture = new Fixture()
                .CustomizePoint();
            _streetNames = _fixture.Create<IEnumerable<StreetName>>();
        }

        [Theory]
        [InlineData("kerkstraat", "kerkstraat")]
        [InlineData("Kerkstraat", "Kerkstraat")]
        [InlineData("DeStRaat", "dEsTraAt")]
        [InlineData("kerkstraat", "kérkstraat")]
        [InlineData("Kerkstraat", "Kérkstraat")]
        [InlineData("DeStRaat", "désTraAt")]
        [InlineData("Josephina De Bakker-Breugelmansstraat", "J. DE BAKKER-BREUGELMANSSTRAAT")]
        [InlineData("Alfons Van den Sandelaan", "ALF.VAN DEN SANDELAAN")]
        [InlineData("Jan Baptist Rampelbergstraat", "J.B. RAMPELBERGSTRAAT")]
        [InlineData("Lodewijk Engelbertus van Arenbergplein", "LODEWIJK ENG. VAN ARENBERGPLEIN")]
        [InlineData("J. Van Boendaelelaan", "J.V. BOENDAELEL.RES. BOENDAELE")]
        [InlineData("Bronplein", "BRONPLEIN RES.GERTRUDIS")]
        [InlineData("Hannuitsesteenweg", "HANNUITSESTWG. RES. HEMELRIJCK")]
        [InlineData("D'Urselstraat", "D'Urselstraat")]
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
                null,
                string.Empty,
                string.Empty,
                null);
            var streetNames = _streetNames.Concat(new[] { expectedStreetName });
            var matcher = new StreetNameMatcher(streetNames);

            var matched = matcher.MatchStreetName(_fixture.Create<string>(), search).FirstOrDefault();

            matched.Should().BeEquivalentTo(expectedStreetName);
        }

        [Theory]
        [InlineData("kerkstraat", "kerkstraat")]
        [InlineData("Kerkstraat", "Kerkstraat")]
        [InlineData("DeStRaat", "dEsTraAt")]
        [InlineData("kerkstraat", "kérkstraat")]
        [InlineData("Kerkstraat", "Kérkstraat")]
        [InlineData("DeStRaat", "désTraAt")]
        [InlineData("D'Urselstraat", "D'Urselstraat")]
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
                null,
                string.Empty,
                string.Empty,
                null);
            var streetNames = _streetNames.Concat(new[] { expectedStreetName });
            var matcher = new StreetNameMatcher(streetNames);

            var matched = matcher.MatchStreetName(_fixture.Create<string>(), search).FirstOrDefault();

            matched.Should().BeEquivalentTo(expectedStreetName);
        }

        [Theory]
        [InlineData("kerkstraat", "kerkstraat")]
        [InlineData("Kerkstraat", "Kerkstraat")]
        [InlineData("DeStRaat", "dEsTraAt")]
        [InlineData("kerkstraat", "kérkstraat")]
        [InlineData("Kerkstraat", "Kérkstraat")]
        [InlineData("DeStRaat", "désTraAt")]
        [InlineData("D'Urselstraat", "D'Urselstraat")]
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
                null,
                string.Empty,
                string.Empty,
                null);
            var streetNames = _streetNames.Concat(new[] { expectedStreetName });
            var matcher = new StreetNameMatcher(streetNames);

            var matched = matcher.MatchStreetName(_fixture.Create<string>(), search).FirstOrDefault();

            matched.Should().BeEquivalentTo(expectedStreetName);
        }

        [Theory]
        [InlineData("kerkstraat", "kerkstraat")]
        [InlineData("Kerkstraat", "Kerkstraat")]
        [InlineData("DeStRaat", "dEsTraAt")]
        [InlineData("kerkstraat", "kérkstraat")]
        [InlineData("Kerkstraat", "Kérkstraat")]
        [InlineData("DeStRaat", "désTraAt")]
        [InlineData("D'Urselstraat", "D'Urselstraat")]
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
                null,
                string.Empty,
                string.Empty,
                null);
            var streetNames = _streetNames.Concat(new[] { expectedStreetName });
            var matcher = new StreetNameMatcher(streetNames);

            var matched = matcher.MatchStreetName(_fixture.Create<string>(), search).FirstOrDefault();

            matched.Should().BeEquivalentTo(expectedStreetName);
        }
    }
}
