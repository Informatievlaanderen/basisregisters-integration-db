namespace Basisregisters.IntegrationDb.Bosa.Infrastructure.Options
{
    public class FullDownloadOptions
    {
        public required string UploadBucket { get; set; }
        public required string FileNameFormat { get; set; }
        public bool UploadToFtp { get; set; }
        public required string FtpFolder { get; set; }
    }
}
