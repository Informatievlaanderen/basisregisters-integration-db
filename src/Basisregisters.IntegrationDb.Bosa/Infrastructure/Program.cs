namespace Basisregisters.IntegrationDB.Bosa.Infrastructure
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Amazon.Runtime;
    using Amazon.S3;
    using Amazon.SimpleNotificationService;
    using Be.Vlaanderen.Basisregisters.Aws.DistributedMutex;
    using Be.Vlaanderen.Basisregisters.BlobStore;
    using Be.Vlaanderen.Basisregisters.BlobStore.Aws;
    using Be.Vlaanderen.Basisregisters.GrAr.Notifications;
    using Destructurama;
    using FluentFTP;
    using IntegrationDb.Bosa;
    using IntegrationDb.Bosa.Infrastructure.Options;
    using IntegrationDb.Bosa.Repositories;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
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

                    services.AddTransient<IPostalInfoRepository, PostalInfoRepository>(_ => new PostalInfoRepository(connectionString));
                    services.AddTransient<IRegistryService, PostalInfoService>();

                    services.AddTransient<IMunicipalityRepository, MunicipalityRepository>(_ => new MunicipalityRepository(connectionString));
                    services.AddTransient<IRegistryService, MunicipalityService>();

                    services.AddTransient<IStreetNameRepository, StreetNameRepository>(_ => new StreetNameRepository(connectionString));
                    services.AddTransient<IRegistryService, StreetNameService>();

                    services.AddTransient<IAddressRepository, AddressRepository>(_ => new AddressRepository(connectionString));
                    services.AddTransient<IRegistryService, AddressService>();

                    services.AddSingleton<IClock>(_ => SystemClock.Instance);

                    services.AddAWSService<IAmazonSimpleNotificationService>();
                    services.AddSingleton<INotificationService>(sp =>
                        new NotificationService(sp.GetRequiredService<IAmazonSimpleNotificationService>(),
                            hostContext.Configuration.GetValue<string>("TopicArn")));

                    services.Configure<S3Options>(hostContext.Configuration.GetSection("S3"));
                    services.Configure<FullDownloadOptions>(hostContext.Configuration);
                    services.AddSingleton<IBlobClient>(sp =>
                    {
                        var options = sp.GetRequiredService<IOptions<FullDownloadOptions>>().Value;
                        var s3Options = sp.GetRequiredService<IOptions<S3Options>>().Value;

                        var s3Client = !string.IsNullOrEmpty(s3Options.ServiceUrl)
                            ? new AmazonS3Client(new BasicAWSCredentials(s3Options.AccessKey, s3Options.SecretKey),
                                new AmazonS3Config
                                {

                                    ServiceURL = s3Options.ServiceUrl,
                                    DisableHostPrefixInjection = true,
                                    ForcePathStyle = true,
                                    LogResponse = true
                                })
                            : new AmazonS3Client();

                        return new S3BlobClient(s3Client, options.UploadBucket);
                    });

                    services.Configure<FtpOptions>(hostContext.Configuration.GetSection("Ftp"));
                    services.AddSingleton<IAsyncFtpClient>(sp =>
                    {
                        var options = sp.GetRequiredService<IOptions<FtpOptions>>().Value;

                        return new AsyncFtpClient(options.Host, options.Username, options.Password, options.Port ?? 0);
                    });

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
