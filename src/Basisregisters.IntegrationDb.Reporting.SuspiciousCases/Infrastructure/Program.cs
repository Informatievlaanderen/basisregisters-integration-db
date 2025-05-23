namespace Basisregisters.IntegrationDb.Reporting.SuspiciousCases.Infrastructure
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Amazon.SimpleNotificationService;
    using Be.Vlaanderen.Basisregisters.Aws.DistributedMutex;
    using Be.Vlaanderen.Basisregisters.GrAr.Notifications;
    using Destructurama;
    using IntegrationDb.Reporting.SuspiciousCases.Infrastructure.Options;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using NodaTime;
    using Serilog;
    using Serilog.Debugging;

    public sealed class Program
    {
        private Program()
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

            Log.Information("Initializing IntegrationDb.Reporting.SuspiciousCases");

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

                    services.AddSingleton<IClock>(_ => SystemClock.Instance);

                    services.AddAWSService<IAmazonSimpleNotificationService>();
                    services.AddSingleton<INotificationService>(sp =>
                        new NotificationService(sp.GetRequiredService<IAmazonSimpleNotificationService>(),
                            hostContext.Configuration.GetValue<string>("TopicArn")!));

                    services.Configure<AzureBlobOptions>(hostContext.Configuration.GetSection("AzureBlob"));

                })
                .UseConsoleLifetime()
                .Build();

            Log.Information("Starting IntegrationDb.Reporting.SuspiciousCases");

            var logger = host.Services.GetRequiredService<ILogger<Program>>();
            var configuration = host.Services.GetRequiredService<IConfiguration>();

            try
            {
                await DistributedLock<Program>.RunAsync(
                    async () =>
                    {
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
