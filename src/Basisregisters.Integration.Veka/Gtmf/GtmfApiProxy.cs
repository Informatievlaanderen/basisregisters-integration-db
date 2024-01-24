namespace Basisregisters.Integration.Veka.Gtmf
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Configuration;
    using Microsoft.Extensions.Options;
    using IdentityModel;
    using IdentityModel.Client;
    using Newtonsoft.Json;

    public interface IGtmfApiProxy
    {
        Task<IEnumerable<MeldingEvent>> GetMeldingEventsFrom(int lastPosition);
        Task<Melding> GetMelding(string id);
    }

    public class GtmfApiProxy : IGtmfApiProxy
    {
        private AccessToken? _accessToken;

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly VekaOptions _vekaOptions;
        private readonly GtmfApiOptions _gtmfApiOptions;

        public GtmfApiProxy(
            IHttpClientFactory httpClientFactory,
            IOptions<VekaOptions> vekaOptions,
            IOptions<GtmfApiOptions> gtmfApiOptions)
        {
            _httpClientFactory = httpClientFactory;
            _vekaOptions = vekaOptions.Value;
            _gtmfApiOptions = gtmfApiOptions.Value;
        }

        public async Task<IEnumerable<MeldingEvent>> GetMeldingEventsFrom(int lastPosition)
        {
            var accessToken = await GetAccessToken();

            using var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var requestUri =
                $"{_gtmfApiOptions.BaseUrl.TrimEnd('/')}/api/v1/meldingevents?hasmeldingmodel=GRAR&istype=MeldingAfgerondEvent&after_id={lastPosition}&limit=100";
            var response = await httpClient.GetAsync(requestUri);

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var events = JsonConvert.DeserializeObject<MeldingEventsResponse>(responseContent);

            return events!.Events
                .Select(x => new MeldingEvent(x.Position, x.MeldingId, x.Type))
                .ToList();
        }

        public async Task<Melding> GetMelding(string id)
        {
            var accessToken = await GetAccessToken();

            using var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var v1Melding = await GetMeldingV1(id, httpClient);

            if (!v1Melding!.GetIndienerId().Equals(_vekaOptions.AgentId, StringComparison.InvariantCultureIgnoreCase))
            {
                return Melding.NietVekaMelding(id);
            }

            var v2Melding = await GetMeldingV2(id, httpClient);

            var behandelaar = await GetAgentOrganisatie(v1Melding.GetBehandelaarUri(), httpClient);

            return Melding.VekaMelding(
                id,
                v2Melding!.ReferentieMelder,
                v1Melding.Referentie,
                v1Melding.GetBeschrijving(),
                behandelaar!.Naam,
                v1Melding.GetStatus(),
                v1Melding.GetToelichtingBehandelaar(),
                v1Melding.GetIndieningsdatum());
        }

        private async Task<MeldingV1Response?> GetMeldingV1(string id, HttpClient httpClient)
        {
            var v1RequestUri = $"{_gtmfApiOptions.BaseUrl.TrimEnd('/')}/api/v1/meldingen/{id}";
            var v1Response = await httpClient.GetAsync(v1RequestUri);

            v1Response.EnsureSuccessStatusCode();

            var v1ResponseContent = await v1Response.Content.ReadAsStringAsync();
            var v1Melding = JsonConvert.DeserializeObject<MeldingV1Response>(v1ResponseContent);
            return v1Melding;
        }

        private async Task<MeldingV2Response?> GetMeldingV2(string id, HttpClient httpClient)
        {
            var v2RequestUri = $"{_gtmfApiOptions.BaseUrl.TrimEnd('/')}/api/v2/meldingen/{id}";
            var v2Response = await httpClient.GetAsync(v2RequestUri);

            v2Response.EnsureSuccessStatusCode();

            var v2ResponseContent = await v2Response.Content.ReadAsStringAsync();
            var v2Melding = JsonConvert.DeserializeObject<MeldingV2Response>(v2ResponseContent);
            return v2Melding;
        }

        private static async Task<AgentOrganisatieResponse?> GetAgentOrganisatie(
            string behandelaarRequestUri, HttpClient httpClient)
        {
            var behandelaarResponse = await httpClient.GetAsync(behandelaarRequestUri);

            behandelaarResponse.EnsureSuccessStatusCode();

            var behandelaarResponseContent = await behandelaarResponse.Content.ReadAsStringAsync();
            var behandelaar = JsonConvert.DeserializeObject<AgentOrganisatieResponse>(behandelaarResponseContent);
            return behandelaar;
        }

        private async Task<string> GetAccessToken()
        {
            if (_accessToken is not null && !_accessToken.IsExpired)
            {
                return _accessToken.Token;
            }

            var tokenClient = new TokenClient(
                () => _httpClientFactory.CreateClient(),
                new TokenClientOptions
                {
                    Address = _gtmfApiOptions.TokenEndpoint,
                    ClientId = _gtmfApiOptions.ClientId,
                    ClientSecret = _gtmfApiOptions.ClientSecret,
                    Parameters = new Parameters(new[] { new KeyValuePair<string, string>("scope", "GTMF_Bro_GRAR_Alle") })
                });

            var response = await tokenClient.RequestTokenAsync(OidcConstants.GrantTypes.ClientCredentials);

            _accessToken = new AccessToken(response.AccessToken, response.ExpiresIn);

            return response.AccessToken;
        }
    }

    public class AccessToken
    {
        private readonly DateTime _expiresAt;

        public string Token { get; }

        // Let's regard it as expired 30 seconds before it actually expires.
        public bool IsExpired => _expiresAt < DateTime.Now.Add(TimeSpan.FromSeconds(10));

        public AccessToken(string token, int expiresIn)
        {
            _expiresAt = DateTime.Now.AddSeconds(expiresIn);
            Token = token;
        }
    }
}
