namespace Basisregisters.IntegrationDb.Consumer.Address
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Serilog;

    public sealed class Program
    {
        public const string ConsumerName = "Address";

        private Program()
        { }

        public static async Task Main(string[] args)
        {
            Log.Information($"Initializing {ConsumerName} Consumer");

            var host = new HostBuilder()
                    .CreateConsumerHost<BackgroundTask>(args)
                    .UseConsoleLifetime()
                    .Build();

            Log.Information($"Starting {ConsumerName} Consumer");

            var logger = host.Services.GetRequiredService<ILogger<Program>>();

            await host.RunAsync(logger);
        }
    }
}
