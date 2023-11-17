namespace Basisregisters.IntegrationDb.Consumer
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.MessageHandling.Kafka;
    using Be.Vlaanderen.Basisregisters.MessageHandling.Kafka.Consumer;
    using Destructurama;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Schema;
    using Serilog;
    using Serilog.Debugging;
    using Serilog.Extensions.Logging;

    public static class HostBuilderExtensions
    {
        public static HostBuilder CreateConsumerHost<TBackgroundService>(this HostBuilder hb, string[] args)
        where TBackgroundService : BackgroundService
        {
            AppDomain.CurrentDomain.FirstChanceException += (_, eventArgs) =>
                Log.Debug(
                    eventArgs.Exception,
                    "FirstChanceException event raised in {AppDomain}.",
                    AppDomain.CurrentDomain.FriendlyName);

            AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
                Log.Fatal((Exception) eventArgs.ExceptionObject, "Encountered a fatal exception, exiting program.");

            hb.ConfigureAppConfiguration((_, builder) =>
                {
                    builder
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                        .AddJsonFile($"appsettings.development.json", optional: true, reloadOnChange: false)
                        .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true, reloadOnChange: false)
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
                    var connectionString = hostContext.Configuration.GetConnectionString("IntegrationDbAdmin")
                                           ?? throw new InvalidOperationException(
                                               $"Could not find a connection string with name 'IntegrationDbAdmin'");

                    services
                        .AddDbContext<IntegrationContext>((_, options) =>
                        {
                            options.UseLoggerFactory(new SerilogLoggerFactory(Log.Logger));
                            options.UseNpgsql(connectionString, sqlServerOptions =>
                            {
                                sqlServerOptions.EnableRetryOnFailure();
                                sqlServerOptions.MigrationsHistoryTable(IntegrationContext.MigrationsTableName, IntegrationContext.Schema);
                                sqlServerOptions.UseNetTopologySuite();
                            });
                        });
                    var loggerFactory = new SerilogLoggerFactory(Log.Logger);

                    var bootstrapServers = hostContext.Configuration["Kafka:BootstrapServers"];
                    var topic = $"{hostContext.Configuration["Topic"]}" ?? throw new ArgumentException("Configuration has no Topic.");
                    var consumerGroupId = hostContext.Configuration["GroupId"] ?? throw new ArgumentException("Configuration has no GroupId.");

                    var consumerOptions = new ConsumerOptions(
                        new BootstrapServers(bootstrapServers),
                        new Topic(topic),
                        new ConsumerGroupId(consumerGroupId),
                        new JsonSerializerSettings
                            { });
                    //EventsJsonSerializerSettingsProvider.CreateSerializerSettings());

                    consumerOptions.ConfigureSaslAuthentication(new SaslAuthentication(
                        hostContext.Configuration["Kafka:SaslUserName"],
                        hostContext.Configuration["Kafka:SaslPassword"]));

                    services
                        .AddSingleton<IKafkaConsumer>((_) => new KafkaConsumer(consumerOptions))
                        .AddHostedService<TBackgroundService>();
                });

            return hb;
        }

        public static async Task RunAsync<T>(this IHost host, ILogger<T> logger)
        {
            try
            {
                await host.RunAsync().ConfigureAwait(false);
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
