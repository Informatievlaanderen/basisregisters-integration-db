namespace Basisregisters.IntegrationDB.Bosa.Infrastructure
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Aws.DistributedMutex;
    using Destructurama;
    using IntegrationDb.Bosa;
    using IntegrationDb.Bosa.Repositories;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using NodaTime;
    using Serilog;
    using Serilog.Debugging;

    public sealed class Program
    {
        protected Program()
        { }

        public static async Task Main(string[] args)
        {
            AppDomain.CurrentDomain.FirstChanceException += (_, eventArgs) =>
                Log.Debug(
                    eventArgs.Exception,
                    "FirstChanceException event raised in {AppDomain}.",
                    AppDomain.CurrentDomain.FriendlyName);

            AppDomain.CurrentDomain.UnhandledException += (_, eventArgs) =>
                Log.Fatal((Exception)eventArgs.ExceptionObject, "Encountered a fatal exception, exiting program.");

            Log.Information("Initializing IntegrationDb.Bosa");

            var host = new HostBuilder()
                .ConfigureAppConfiguration((_, builder) =>
                {
                    builder
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                        .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true,
                            reloadOnChange: false)
                        .AddEnvironmentVariables()
                        .AddCommandLine(args);
                })
                .ConfigureLogging((hostContext, builder) =>
                {
                    SelfLog.Enable(Console.WriteLine);

                    Log.Logger = new LoggerConfiguration() //NOSONAR logging configuration is safe
                        .ReadFrom.Configuration(hostContext.Configuration)
                        .Enrich.FromLogContext()
                        .Enrich.WithMachineName()
                        .Enrich.WithThreadId()
                        .Enrich.WithEnvironmentUserName()
                        .Destructure.JsonNetTypes()
                        .CreateLogger();

                    builder.ClearProviders();
                    builder.AddSerilog(Log.Logger);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    var connectionString = hostContext.Configuration.GetConnectionString("Integration")
                                           ?? throw new ArgumentNullException("hostContext.Configuration.GetConnectionString(\"Integration\")");

                    services.AddSingleton<DatabaseSetup>(_ => new DatabaseSetup(connectionString));

                    services.AddTransient<IPostalInfoRepository, PostalInfoRepository>(_ => new PostalInfoRepository(connectionString));
                    services.AddTransient<IRegistryService, PostalInfoService>();

                    services.AddTransient<IMunicipalityRepository, MunicipalityRepository>(_ => new MunicipalityRepository(connectionString));
                    services.AddTransient<IRegistryService, MunicipalityService>();

                    services.AddTransient<IStreetNameRepository, StreetNameRepository>(_ => new StreetNameRepository(connectionString));
                    services.AddTransient<IRegistryService, StreetNameService>();

                    services.AddTransient<IAddressRepository, AddressRepository>(_ => new AddressRepository(connectionString));
                    services.AddTransient<IRegistryService, AddressService>();

                    services.AddSingleton<IClock>(_ => SystemClock.Instance);

                    services.AddHostedService<FullDownloadService>();
                })
                .UseConsoleLifetime()
                .Build();

            Log.Information("Starting IntegrationDb.Bosa");

            var logger = host.Services.GetRequiredService<ILogger<Program>>();
            var configuration = host.Services.GetRequiredService<IConfiguration>();

            try
            {
                await DistributedLock<Program>.RunAsync(
                    async () =>
                    {
                        host.Services.GetRequiredService<DatabaseSetup>().CheckIfDataPresent();

                        await host.RunAsync().ConfigureAwait(false);
                    },
                    DistributedLockOptions.LoadFromConfiguration(configuration),
                    logger)
                .ConfigureAwait(false);
            }
            catch (AggregateException aggregateException)
            {
                foreach (var innerException in aggregateException.InnerExceptions)
                {
                    logger.LogCritical(innerException, "Encountered a fatal exception, exiting program.");
                }
            }
            catch (Exception e)
            {
                logger.LogCritical(e, "Encountered a fatal exception, exiting program.");
                await Log.CloseAndFlushAsync();

                // Allow some time for flushing before shutdown.
                await Task.Delay(500, default);
                throw;
            }
            finally
            {
                logger.LogInformation("Stopping...");
            }
        }
    }
}
