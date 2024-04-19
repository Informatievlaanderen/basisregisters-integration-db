namespace Basisregisters.IntegrationDb.Bosa.Infrastructure.Options
{
    public class FullDownloadOptions
    {
        public string UploadBucket { get; set; }
        public bool UploadToFtp { get; set; }
        public FtpCredentials FtpCredentials { get; set; }
    }

    public class FtpCredentials
    {
        public string Url { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
