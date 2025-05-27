namespace Basisregisters.IntegrationDb.Reporting.SuspiciousCases.Infrastructure.Options
{
    public class AzureBlobOptions
    {   public required string ContainerName { get; set; }
        public required string ConnectionString { get; set; }
    }
}
