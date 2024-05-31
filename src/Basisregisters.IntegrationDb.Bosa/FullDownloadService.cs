namespace Basisregisters.IntegrationDb.Bosa
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.BlobStore;
    using Be.Vlaanderen.Basisregisters.GrAr.Notifications;
    using FluentFTP;
    using Infrastructure.Options;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    public class FullDownloadService : BackgroundService
    {
        private readonly IEnumerable<IRegistryService> _registryServices;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly IBlobClient _blobClient;
        private readonly IAsyncFtpClient _asyncFtpClient;
        private readonly INotificationService _notificationService;
        private readonly FullDownloadOptions _options;
        private readonly ILogger<FullDownloadService> _logger;

        public FullDownloadService(
            IEnumerable<IRegistryService> registryServices,
            IHostApplicationLifetime hostApplicationLifetime,
            IOptions<FullDownloadOptions> options,
            IBlobClient blobClient,
            IAsyncFtpClient asyncFtpClient,
            INotificationService notificationService,
            ILoggerFactory loggerFactory)
        {
            _registryServices = registryServices;
            _hostApplicationLifetime = hostApplicationLifetime;
            _blobClient = blobClient;
            _asyncFtpClient = asyncFtpClient;
            _notificationService = notificationService;
            _options = options.Value;
            _logger = loggerFactory.CreateLogger<FullDownloadService>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var fullZipFileName = string.Format(_options.FileNameFormat, DateTimeOffset.Now.ToString("yyyyMMdd"));

                var zipExists = await _blobClient.BlobExistsAsync(new BlobName(fullZipFileName), stoppingToken);
                if (zipExists)
                {
                    throw new InvalidOperationException($"Blob with name '{fullZipFileName}' already exists in bucket '{_options.UploadBucket}'");
                }

                using var fullZipStream = new MemoryStream();
                using var fullZipArchive = new ZipArchive(fullZipStream, ZipArchiveMode.Create, leaveOpen: true);

                foreach (var registryService in _registryServices)
                {
                    _logger.LogInformation($"Creating zip {registryService.GetZipFileName()}");

                    var zipFileName = registryService.GetZipFileName();
                    await using var registryZipEntryStream = fullZipArchive.CreateEntry(zipFileName).Open();

                    await CreateRegistryZipArchiveStream(
                        registryService,
                        registryZipEntryStream);

                    await registryZipEntryStream.FlushAsync(stoppingToken);
                }

                await fullZipStream.FlushAsync(stoppingToken);

                fullZipStream.Seek(0, SeekOrigin.Begin);
                await _blobClient.CreateBlobAsync(new BlobName(fullZipFileName),
                    Metadata.None.Add(new KeyValuePair<MetadataKey, string>(new MetadataKey("FileName"), fullZipFileName)),
                    ContentType.Parse("application/zip"),
                    fullZipStream,
                    stoppingToken);

                if (_options.UploadToFtp)
                {
                    await UploadToFtp(fullZipStream, fullZipFileName, stoppingToken);
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

        private async Task UploadToFtp(Stream stream, string fileName, CancellationToken cancellationToken)
        {
            await _asyncFtpClient.AutoConnect(cancellationToken);

            stream.Seek(0, SeekOrigin.Begin);
            var uploadStatus = await _asyncFtpClient.UploadStream(stream, $"{_options.FtpFolder}/{fileName}", token: cancellationToken);

            await _asyncFtpClient.Disconnect(cancellationToken);

            if (uploadStatus != FtpStatus.Success)
            {
                throw new Exception($"Upload to FTP failed with status '{uploadStatus}'");
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
