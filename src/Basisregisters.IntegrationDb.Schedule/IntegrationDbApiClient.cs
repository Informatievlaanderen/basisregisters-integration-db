namespace Basisregisters.IntegrationDb.Schedule;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Be.Vlaanderen.Basisregisters.Auth.AcmIdm;
using Duende.IdentityModel;
using Duende.IdentityModel.Client;
using Infrastructure;
using Microsoft.Extensions.Options;

public class IntegrationDbApiClient
{
    private AccessToken? _accessToken;

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IntegrationDbApiOptions _options;

    public IntegrationDbApiClient(IHttpClientFactory httpClientFactory, IOptions<IntegrationDbApiOptions> options)
    {
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
    }

    public async Task CorrectAddressesDerivedFromBuildingUnitPosition(CancellationToken cancellationToken)
    {
        var accessToken = await GetAccessToken();

        var httpClient = _httpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await httpClient.PostAsync($"{_options.BaseUrl.TrimEnd('/')}/v2/adressen/corrigeren/afgeleid-van-gebouweenheid-posities", null, cancellationToken: cancellationToken);
        response.EnsureSuccessStatusCode();
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
                Address = _options.TokenEndpoint,
                ClientId = _options.ClientId,
                ClientSecret = _options.ClientSecret,
                Parameters = new Parameters([new KeyValuePair<string, string>("scope", Scopes.DvArAdresUitzonderingen)])
            });

        var response = await tokenClient.RequestTokenAsync(OidcConstants.GrantTypes.ClientCredentials);
        if (string.IsNullOrEmpty(response.AccessToken))
        {
            throw new Exception($"Could not get access token: {response.Error}");
        }

        _accessToken = new AccessToken(response.AccessToken, response.ExpiresIn);

        return response.AccessToken;
    }

    private sealed class AccessToken
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
