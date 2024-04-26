namespace Basisregisters.IntegrationDb.Bosa.Infrastructure.Options
{
    public class FullDownloadOptions
    {
        public string UploadBucket { get; set; }
        public string FileNameFormat { get; set; }
        public bool UploadToFtp { get; set; }
        public string FtpFolder { get; set; }
    }
}
