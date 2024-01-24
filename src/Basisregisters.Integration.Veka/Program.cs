namespace Basisregisters.Integration.Veka
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Amazon.DynamoDBv2;
    using Amazon.SimpleNotificationService;
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Be.Vlaanderen.Basisregisters.Aws.DistributedMutex;
    using Be.Vlaanderen.Basisregisters.DataDog.Tracing.Autofac;
    using Configuration;
    using Destructurama;
    using Gtmf;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Notifications;
    using Serilog;
    using Serilog.Debugging;
    using Serilog.Extensions.Logging;

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

            Log.Information("Starting Integration.Veka");

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
                    var loggerFactory = new SerilogLoggerFactory(Log.Logger);

                    services.Configure<VekaOptions>(hostContext.Configuration.GetSection("Veka"));
                    services.Configure<EmailOptions>(hostContext.Configuration.GetSection("Email"));
                    services.Configure<GtmfApiOptions>(hostContext.Configuration.GetSection("GtmfApi"));
                    services.Configure<DistributedLockOptions>(hostContext.Configuration.GetSection("DistributedLock"));

                    services.AddHttpClient();

                    if (hostContext.Configuration.GetValue<string>("DOTNET_ENVIRONMENT") == "Development")
                    {
                        services.AddSingleton<IProjectionState, FakeProjectionState>();
                    }
                    else
                    {
                        services.AddSingleton<IProjectionState, DynamoDbProjectionState>();
                    }
                    services.AddSingleton<IAmazonDynamoDB, AmazonDynamoDBClient>();
                    services.AddSingleton<IGtmfApiProxy, GtmfApiProxy>();
                    services.AddSingleton<IEmailSender, EmailSender>();

                    services.AddAWSService<IAmazonSimpleNotificationService>();
                    services.AddScoped<INotificationService>(provider =>
                    {
                        var snsService = provider.GetRequiredService<IAmazonSimpleNotificationService>();
                        var topicArn = hostContext.Configuration["TopicArn"];
                        return new NotificationService(snsService, topicArn);
                    });
                })
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureContainer<ContainerBuilder>((hostContext, builder) =>
                {
                    var services = new ServiceCollection();

                    builder
                        .RegisterModule(new DataDogModule(hostContext.Configuration));

                    builder
                        .RegisterType<GtmfConsumer>()
                        .As<IHostedService>()
                        .SingleInstance();

                    builder.Populate(services);
                })
                .UseConsoleLifetime()
                .Build();

            var logger = host.Services.GetRequiredService<ILogger<Program>>();
            var loggerFactory = host.Services.GetRequiredService<ILoggerFactory>();
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
