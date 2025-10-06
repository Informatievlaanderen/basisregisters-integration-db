namespace Basisregisters.IntegrationDb.Api.Address;

using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Repositories;

public class AddressModule : Module
{
    public AddressModule(
        IConfiguration configuration,
        IServiceCollection services)
    {
        services
            .Configure<AddressOptions>(configuration.GetSection("AddressOptions"))
            .AddScoped<AddressRepository>(_ => new AddressRepository(configuration.GetConnectionString("Integration")!))
            .AddHttpClient()
            .AddScoped<AddressRegistryApiClient>()
            ;
    }
}
