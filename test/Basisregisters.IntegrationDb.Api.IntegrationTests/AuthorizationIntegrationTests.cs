namespace Basisregisters.IntegrationDb.Api.IntegrationTests
{
    using System.Net;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Auth.AcmIdm;
    using Xunit;

    public class IntegrationTests : IClassFixture<IntegrationTestFixture>
    {
        private readonly IntegrationTestFixture _fixture;

        public IntegrationTests(IntegrationTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Theory]
        [InlineData("/v2/verdachte-gevallen", Scopes.DvArAdresBeheer)]
        [InlineData("/v2/verdachte-gevallen/1", Scopes.DvArAdresBeheer)]
        public async Task ReturnsSuccess(string endpoint, string requiredScopes)
        {
            var client = _fixture.TestServer.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await _fixture.GetAccessToken(requiredScopes));

            var response = await client.GetAsync(endpoint, CancellationToken.None);
            Assert.NotNull(response);
            Assert.NotEqual(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.NotEqual(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Theory]
        [InlineData("/v2/verdachte-gevallen")]
        [InlineData("/v2/verdachte-gevallen/1")]
        public async Task ReturnsUnauthorized(string endpoint)
        {
            var client = _fixture.TestServer.CreateClient();

            var response = await client.GetAsync(endpoint, CancellationToken.None);
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Theory]
        [InlineData("/v2/verdachte-gevallen")]
        [InlineData("/v2/verdachte-gevallen", Scopes.VoInfo)]
        [InlineData("/v2/verdachte-gevallen/1")]
        [InlineData("/v2/verdachte-gevallen/1", Scopes.VoInfo)]
        public async Task ReturnsForbidden(string endpoint, string scope = "")
        {
            var client = _fixture.TestServer.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await _fixture.GetAccessToken(scope));

            var response = await client.GetAsync(endpoint, CancellationToken.None);
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }
}
