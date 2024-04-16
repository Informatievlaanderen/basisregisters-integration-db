namespace Basisregisters.IntegrationDb.Bosa
{
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;

    public class FullDownloadService(IEnumerable<IRegistryService> registryServices) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var fullZipStream = new MemoryStream();
            using var fullZipArchive = new ZipArchive(fullZipStream, ZipArchiveMode.Create, leaveOpen: true);

            foreach (var registryService in registryServices)
            {
                var zipFileName = registryService.GetZipFileName();
                await using var registryZipEntryStream = fullZipArchive.CreateEntry(zipFileName).Open();

                await CreateRegistryZipArchiveStream(
                    registryService,
                    registryZipEntryStream);
            }

            // Todo: place full zip on FTP
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
