namespace Basisregisters.IntegrationDb.Gtmf.Meldingen.Api
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Configuration;
    using Events;
    using Duende.IdentityModel;
    using Duende.IdentityModel.Client;
    using Meldingen;
    using Microsoft.Extensions.Options;
    using NetTopologySuite;
    using NetTopologySuite.Geometries;
    using NetTopologySuite.IO;
    using Newtonsoft.Json;
    using NodaTime;

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
        private readonly WKTReader _wktReader;

        public GtmfApiProxy(
            IHttpClientFactory httpClientFactory,
            IOptions<GtmfApiOptions> gtmfApiOptions)
        {
            _httpClientFactory = httpClientFactory;
            _gtmfApiOptions = gtmfApiOptions.Value;
            _wktReader = new WKTReader(NtsGeometryServices.Instance);
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
            var thema = await GetThema(v1Melding.GetThema(), httpClient);
            var oorzaak = await GetOorzaak(v1Melding.GetOorzaak(), httpClient);
            var geometrie = GetGeometrie(v1Melding.GetGeometrie());

            var meldingsobject = new Meldingsobject(
                v1Melding.GetMeldingsobjectId(),
                Guid.Parse(meldingId),
                Instant.FromDateTimeOffset(v2Melding.DatumIndiening),
                Instant.FromDateTimeOffset(v1Melding.GetDatumVaststelling()),
                indienerOrganisatie.Id,
                v2Melding.Meldingsapplicatie,
                v2Melding.Referentie,
                v2Melding.ReferentieMelder,
                v1Melding.GetOnderwerp(),
                v1Melding.GetBeschrijving(),
                v2Melding.Samenvatting,
                thema,
                oorzaak,
                v1Melding.GetOvoCode(),
                geometrie
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

        private Dictionary<string, string> _cachedThemas = new();
        private async Task<string> GetThema(string id, HttpClient httpClient)
        {
            if (!_cachedThemas.Any())
            {
                var requestUri = $"{_gtmfApiOptions.BaseUrl.TrimEnd('/')}/api/v1/datasets/GRAR/eigenschappen/{MeldingV1ResponseEigenschap.GRAR_Thema}";
                var response = await httpClient.GetAsync(requestUri);

                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var eigenschap = JsonConvert.DeserializeObject<EigenschapResponse>(responseContent)!;
                _cachedThemas = eigenschap.Items.ToDictionary(
                    x => x.Waarde,
                    x => x.Label);
            }

            return _cachedThemas[id];
        }

        private Dictionary<string, string> _cachedOorzaken = new();
        private async Task<string> GetOorzaak(string id, HttpClient httpClient)
        {
            if (!_cachedOorzaken.Any())
            {
                var requestUri = $"{_gtmfApiOptions.BaseUrl.TrimEnd('/')}/api/v1/datasets/GRAR/eigenschappen/{MeldingV1ResponseEigenschap.GRAR_Oorzaak}";
                var response = await httpClient.GetAsync(requestUri);

                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var eigenschap = JsonConvert.DeserializeObject<EigenschapResponse>(responseContent)!;
                _cachedOorzaken = eigenschap.Items.ToDictionary(
                    x => x.Waarde,
                    x => x.Label);
            }

            return _cachedOorzaken[id];
        }

        private Geometry? GetGeometrie(string? geometrieAsString)
        {
            if (string.IsNullOrWhiteSpace(geometrieAsString))
            {
                return null;
            }

            try
            {
                var geometry = _wktReader.Read(geometrieAsString);

                if (geometry.IsValid)
                {
                    return geometry;
                }
            }
            catch (Exception)
            {
                // ignored
            }

            return null;
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
