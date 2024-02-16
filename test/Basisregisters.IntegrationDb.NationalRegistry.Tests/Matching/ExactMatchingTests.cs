namespace Basisregisters.IntegrationDb.NationalRegistry.Tests.Matching
{
    using System.Collections.Generic;
    using NationalRegistry.Matching;
    using Repositories;
    using Xunit;

    public class ExactMatchingTests
    {
        private StreetNameMatcher sut;

        public ExactMatchingTests()
        {
        }

        [Theory]
        [InlineData("DeStRaat", "dEsTraAt")]
        public void GivenExactStreetNameByDutch(string search, string dutchStreet)
        {
            var streetNames = new List<StreetName>()
            {
                new StreetName(1, dutchStreet, null, null, null, null, null, null, null)
            };
            sut = new StreetNameMatcher(streetNames);

            var match = sut.MatchStreetName(search);
        }
    }
}
