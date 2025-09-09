namespace Basisregisters.IntegrationDb.Gtmf.Meldingen;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SimpleNotificationService.Model;
using Microsoft.Extensions.Options;

public interface IOrganizationApiClient
{
    Task<OrganizationDto> GetByOvoCodeAsync(string ovoCode, CancellationToken cancellationToken);
}

public class OrganizationApiClient : IOrganizationApiClient
{
    private readonly IHttpClientFactory _factory;
    private readonly OrganizationRegistryOptions _options;

    public OrganizationApiClient(IHttpClientFactory factory, IOptions<OrganizationRegistryOptions> options)
    {
        _factory = factory;
        _options = options.Value;
    }

    public async Task<OrganizationDto> GetByOvoCodeAsync(string ovoCode, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_options.BaseUrl))
        {
            throw new InvalidOperationException($"{nameof(_options.BaseUrl)} is not configured");
        }

        var httpClient = _factory.CreateClient();
        var response = await httpClient.GetAsync(CreateUri(ovoCode), cancellationToken);
        response.EnsureSuccessStatusCode();

        var results = await response.Content.ReadFromJsonAsync<IEnumerable<OrganizationDto>?>(cancellationToken: cancellationToken);
        return results?.FirstOrDefault()
            ?? throw new NotFoundException($"{nameof(OrganizationDto)} with OvoCode {ovoCode} not found in Organization Registry");
    }

    private Uri CreateUri(string ovoCode)
    {
        return new Uri(new Uri(_options.BaseUrl), $"/v1/search/organisations?q=ovoNumber:{ovoCode}&fields=name,ovoNumber,kboNumber");
    }
}

public class OrganizationDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string OvoNumber { get; set; }
    public string? KboNumber { get; set; }
}

public class OrganizationRegistryOptions
{
    public string BaseUrl { get; set; }
}
