namespace Basisregisters.IntegrationDb.Bosa
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Threading;
    using System.Threading.Tasks;
    using Basisregisters.IntegrationDb.Bosa.Infrastructure.Options;
    using Be.Vlaanderen.Basisregisters.BlobStore;
    using Be.Vlaanderen.Basisregisters.BlobStore.Aws;
    using Be.Vlaanderen.Basisregisters.GrAr.Notifications;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    public class FullDownloadService : BackgroundService
    {
        private readonly IEnumerable<IRegistryService> _registryServices;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly IBlobClient _blobClient;
        private readonly INotificationService _notificationService;
        private readonly FullDownloadOptions _options;
        private readonly ILogger<FullDownloadService> _logger;

        public FullDownloadService(
            IEnumerable<IRegistryService> registryServices,
            IHostApplicationLifetime hostApplicationLifetime,
            IOptions<FullDownloadOptions> options,
            IBlobClient blobClient,
            INotificationService notificationService,
            ILoggerFactory loggerFactory)
        {
            _registryServices = registryServices;
            _hostApplicationLifetime = hostApplicationLifetime;
            _blobClient = blobClient;
            _notificationService = notificationService;
            _options = options.Value;
            _logger = loggerFactory.CreateLogger<FullDownloadService>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var fullZipFileName = $"Flanders_FD_{DateTimeOffset.Now:yyyyMMdd}_L72";

                var zipExists = await _blobClient.BlobExistsAsync(new BlobName(fullZipFileName), stoppingToken);
                if (zipExists)
                {
                    throw new InvalidOperationException($"Blob with name '{fullZipFileName}' already exists in bucket '{_options.UploadBucket}'");
                }

                using var fullZipStream = new MemoryStream();
                //await using var fullZipStream = new FileStream("C:\\Git\\test.zip", FileMode.Create);
                using var fullZipArchive = new ZipArchive(fullZipStream, ZipArchiveMode.Create, leaveOpen: true);

                foreach (var registryService in _registryServices)
                {
                    _logger.LogInformation($"Creating zip {registryService.GetZipFileName()}");

                    var zipFileName = registryService.GetZipFileName();
                    await using var registryZipEntryStream = fullZipArchive.CreateEntry(zipFileName).Open();

                    await CreateRegistryZipArchiveStream(
                        registryService,
                        registryZipEntryStream);
                }

                await _blobClient.CreateBlobAsync(new BlobName(fullZipFileName),
                    Metadata.None.Add(new KeyValuePair<MetadataKey, string>(new MetadataKey("FileName"), fullZipFileName)),
                    ContentType.Parse("application/zip"),
                    fullZipStream,
                    stoppingToken);

                if (_options.UploadToFtp)
                {
                    // Todo: place full zip on FTP
                }

                await _notificationService.PublishToTopicAsync(new NotificationMessage(
                    "Bosa",
                    $"Bosa zip '{fullZipFileName}' uploaded successfully.",
                    "Bosa full download",
                    NotificationSeverity.Good));

                _hostApplicationLifetime.StopApplication();
            }
            catch (Exception ex)
            {
                await _notificationService.PublishToTopicAsync(new NotificationMessage(
                    "Bosa",
                    $"Bosa zip upload failed: {ex}",
                    "Bosa full download",
                    NotificationSeverity.Danger));

                throw;
            }
        }

        private static async Task CreateRegistryZipArchiveStream(
            IRegistryService registryService,
            Stream outputStream)
        {
            using var zipArchive = new ZipArchive(outputStream, ZipArchiveMode.Create, leaveOpen: true);

            var xmlFileName = registryService.GetXmlFileName();

            await using var xmlEntryStream = zipArchive.CreateEntry(xmlFileName).Open();
            registryService.CreateXml(xmlEntryStream);
        }
    }
}
