namespace Basisregisters.IntegrationDb.Gtmf.Meldingen.Infrastructure
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Amazon.SimpleNotificationService;
    using Api;
    using Api.Events;
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Be.Vlaanderen.Basisregisters.Aws.DistributedMutex;
    using Configuration;
    using Destructurama;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Notifications;
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

            var host = new HostBuilder()
                .ConfigureAppConfiguration((_, builder) =>
                {
                    builder
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                        .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true, reloadOnChange: false)
                        .AddEnvironmentVariables()
                        .AddCommandLine(args);
                })
                .ConfigureLogging((hostContext, builder) =>
                {
                    SelfLog.Enable(Console.WriteLine);

                    Log.Logger = new LoggerConfiguration()
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
                    services.Configure<GtmfApiOptions>(hostContext.Configuration.GetSection("GtmfApi"));
                    services.Configure<DistributedLockOptions>(hostContext.Configuration.GetSection("DistributedLock"));

                    services.AddHttpClient();
                    services.AddSingleton<IGtmfApiProxy, GtmfApiProxy>();
                    services.AddSingleton<IMeldingsobjectEventDeserializer, MeldingsobjectEventDeserializer>();

                    services.AddNpgsql<MeldingenContext>(
                        hostContext.Configuration.GetConnectionString("Integration")!,
                        optionsBuilder =>
                        {
                            optionsBuilder.EnableRetryOnFailure();
                            optionsBuilder.MigrationsHistoryTable(
                                MeldingenContext.MigrationsTableName,
                                MeldingenContext.Schema);
                            optionsBuilder.UseNetTopologySuite();
                        });

                    services.AddAWSService<IAmazonSimpleNotificationService>();
                    services.AddScoped<INotificationService>(provider =>
                    {
                        var snsService = provider.GetRequiredService<IAmazonSimpleNotificationService>();
                        var topicArn = hostContext.Configuration["TopicArn"]!;
                        return new NotificationService(snsService, topicArn);
                    });
                })
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureContainer<ContainerBuilder>((_, builder) =>
                {
                    builder
                        .RegisterType<MeldingEventConsumer>()
                        .As<IHostedService>()
                        .SingleInstance();

                    builder.Populate(new ServiceCollection());
                })
                .UseConsoleLifetime()
                .Build();

            var logger = host.Services.GetRequiredService<ILogger<Program>>();
            var configuration = host.Services.GetRequiredService<IConfiguration>();
            var loggerFactory = host.Services.GetRequiredService<ILoggerFactory>();

            logger.LogInformation("Starting IntegrationDb.Gtmf.Meldingen");

            try
            {
                await DistributedLock<Program>.RunAsync(
                    async () =>
                    {
                        MigrationsHelper.Run(
                            configuration.GetConnectionString("IntegrationAdmin")!,
                            loggerFactory);

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
                Log.CloseAndFlush();

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
