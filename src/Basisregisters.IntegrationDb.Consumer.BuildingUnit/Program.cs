namespace Basisregisters.IntegrationDb.Consumer.BuildingUnit
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Serilog;

    public sealed class Program
    {
        private Program()
        { }

        public static async Task Main(string[] args)
        {
            Log.Information("Initializing BuildingUnit Consumer");

            var host = new HostBuilder()
                .CreateConsumerHost<BackgroundTask>(args)
                .UseConsoleLifetime()
                .Build();

            Log.Information("Starting BuildingUnit Consumer");

            var logger = host.Services.GetRequiredService<ILogger<Program>>();

            await host.RunAsync(logger);
        }
    }
}
