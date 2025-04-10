namespace Basisregisters.IntegrationDb.Bosa.Infrastructure.Options
{
    public class S3Options
    {
        public required string ServiceUrl { get; set; }
        public required string AccessKey { get; set; }
        public required string SecretKey { get; set; }
    }
}
