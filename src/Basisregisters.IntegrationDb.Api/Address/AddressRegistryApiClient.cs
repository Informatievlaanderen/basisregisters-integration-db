namespace Basisregisters.IntegrationDb.Api.Address;

using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using AddressRegistry.Api.BackOffice.Abstractions.Requests;
using Be.Vlaanderen.Basisregisters.AspNetCore.Mvc.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

public class AddressRegistryApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AddressOptions _options;

    public AddressRegistryApiClient(
        IOptions<AddressOptions> options,
        IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
    }

    public async Task CorrectAddressPosition(
        int addressPersistentLocalId,
        CorrectAddressPositionRequest request,
        StringValues? correlationIdHeader,
        StringValues authorizationHeader,
        CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient();
        if(correlationIdHeader is not null)
            client.DefaultRequestHeaders.Add(AddCorrelationIdToResponseMiddleware.HeaderName, correlationIdHeader.ToString());

        var copyHeaders = new[] { "Authorization" };
        foreach (var headerName in copyHeaders)
        {
            client.DefaultRequestHeaders.Add(headerName, authorizationHeader.ToString());
        }

        var response = await client.PostAsJsonAsync($"{_options.BackOfficeApiBaseUrl.TrimEnd('/')}/v2/adressen/{addressPersistentLocalId}/acties/corrigeren/adrespositie", request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
