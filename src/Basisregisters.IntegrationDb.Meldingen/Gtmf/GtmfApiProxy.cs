namespace Basisregisters.IntegrationDb.Meldingen.Gtmf
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Configuration;
    using Events;
    using IdentityModel;
    using IdentityModel.Client;
    using Meldingen;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;

    public interface IGtmfApiProxy
    {
        Task<IEnumerable<MeldingEvent>> GetEventsFrom(int lastPosition);
        Task<IngediendMeldingsobject> GetMeldingsobject(string meldingId);
    }

    public class GtmfApiProxy : IGtmfApiProxy
    {
        private AccessToken? _accessToken;

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly GtmfApiOptions _gtmfApiOptions;

        public GtmfApiProxy(
            IHttpClientFactory httpClientFactory,
            IOptions<GtmfApiOptions> gtmfApiOptions)
        {
            _httpClientFactory = httpClientFactory;
            _gtmfApiOptions = gtmfApiOptions.Value;
        }

        public async Task<IEnumerable<MeldingEvent>> GetEventsFrom(int lastPosition)
        {
            var accessToken = await GetAccessToken();

            using var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var requestUri =
                $"{_gtmfApiOptions.BaseUrl.TrimEnd('/')}/api/v1/meldingevents?hasmeldingmodel=GRAR&after_id={lastPosition}&limit=100";
            var response = await httpClient.GetAsync(requestUri);

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var events = JsonConvert.DeserializeObject<MeldingEventsResponse>(responseContent);

            return events!.Events
                .Select(x => new MeldingEvent(x.Position, x.MeldingId, x.Type, x.Data))
                .ToList();
        }

        public async Task<IngediendMeldingsobject> GetMeldingsobject(string meldingId)
        {
            var accessToken = await GetAccessToken();

            using var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var v1Melding = await GetMeldingV1(meldingId, httpClient);
            var v2Melding = await GetMeldingV2(meldingId, httpClient);

            var indienerOrganisatie = v2Melding.GetIndienerOrganisatie();

            var meldingsobject = new Meldingsobject(
                v1Melding.GetMeldingsobjectId(),
                Guid.Parse(meldingId),
                v2Melding.DatumIndiening,
                v1Melding.GetDatumVaststelling(),
                indienerOrganisatie.Id,
                v2Melding.Meldingsapplicatie,
                v2Melding.Referentie,
                v2Melding.ReferentieMelder,
                v1Melding.GetOnderwerp(),
                v1Melding.GetBeschrijving(),
                v2Melding.Samenvatting,
                v1Melding.GetThema(),
                v1Melding.GetOorzaak(),
                v1Melding.GetOvoCode()
            );

            return new IngediendMeldingsobject(meldingsobject, indienerOrganisatie);
        }

        private async Task<MeldingV1Response> GetMeldingV1(string id, HttpClient httpClient)
        {
            var v1RequestUri = $"{_gtmfApiOptions.BaseUrl.TrimEnd('/')}/api/v1/meldingen/{id}";
            var v1Response = await httpClient.GetAsync(v1RequestUri);

            v1Response.EnsureSuccessStatusCode();

            var v1ResponseContent = await v1Response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<MeldingV1Response>(v1ResponseContent)!;
        }

        private async Task<MeldingV2Response> GetMeldingV2(string id, HttpClient httpClient)
        {
            var v2RequestUri = $"{_gtmfApiOptions.BaseUrl.TrimEnd('/')}/api/v2/meldingen/{id}";
            var v2Response = await httpClient.GetAsync(v2RequestUri);

            v2Response.EnsureSuccessStatusCode();

            var v2ResponseContent = await v2Response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<MeldingV2Response>(v2ResponseContent)!;
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

            _accessToken = new AccessToken(response.AccessToken!, response.ExpiresIn);

            return _accessToken.Token;
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
