namespace Basisregisters.IntegrationDb.Schedule.Infrastructure;

public class IntegrationDbApiOptions
{
    public required string BaseUrl { get; init; }
    public required string TokenEndpoint { get; init; }
    public required string ClientId { get; init; }
    public required string ClientSecret { get; init; }
}
