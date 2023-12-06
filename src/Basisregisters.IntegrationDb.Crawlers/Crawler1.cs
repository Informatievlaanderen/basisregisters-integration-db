namespace Basisregisters.IntegrationDb.Crawlers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Schema;

    public class Crawler1 : BackgroundService
    {
        private readonly IntegrationContext _integrationContext;

        public Crawler1(IntegrationContext integrationContext)
        {
            _integrationContext = integrationContext;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }
    }
}
