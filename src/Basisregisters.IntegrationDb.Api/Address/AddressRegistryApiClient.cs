namespace Basisregisters.IntegrationDb.Api.Address;

using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using AddressRegistry.Api.BackOffice.Abstractions.Requests;
using Be.Vlaanderen.Basisregisters.AspNetCore.Mvc.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

public class AddressRegistryApiClient
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AddressOptions _options;

    public AddressRegistryApiClient(
        IOptions<AddressOptions> options,
        IHttpContextAccessor httpContextAccessor,
        IHttpClientFactory httpClientFactory)
    {
        _httpContextAccessor = httpContextAccessor;
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
    }

    public async Task CorrectAddressPosition(int addressPersistentLocalId, CorrectAddressPositionRequest request, CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add(AddCorrelationIdToResponseMiddleware.HeaderName, _httpContextAccessor.HttpContext!.Response.Headers[AddCorrelationIdToResponseMiddleware.HeaderName].ToString());

        var copyHeaders = new[] { "Authorization" };
        foreach (var headerName in copyHeaders)
        {
            client.DefaultRequestHeaders.Add(headerName, _httpContextAccessor.HttpContext!.Request.Headers[headerName].ToString());
        }

        var response = await client.PostAsJsonAsync($"{_options.BackOfficeApiBaseUrl.TrimEnd('/')}/v2/adressen/{addressPersistentLocalId}/acties/corrigeren/adrespositie", request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
