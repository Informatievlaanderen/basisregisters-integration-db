namespace Basisregisters.IntegrationDb.Bosa
{
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    public class FullDownloadService : BackgroundService
    {
        private readonly IEnumerable<IRegistryService> _registryServices;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly ILogger<FullDownloadService> _logger;

        public FullDownloadService(
            IEnumerable<IRegistryService> registryServices,
            IHostApplicationLifetime hostApplicationLifetime,
            ILoggerFactory loggerFactory)
        {
            _registryServices = registryServices;
            _hostApplicationLifetime = hostApplicationLifetime;
            _logger = loggerFactory.CreateLogger<FullDownloadService>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //using var fullZipStream = new MemoryStream();
            await using var fullZipStream = new FileStream("C:\\Git\\test.zip", FileMode.Create);
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

            // Todo: place full zip on FTP

            _hostApplicationLifetime.StopApplication();
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
