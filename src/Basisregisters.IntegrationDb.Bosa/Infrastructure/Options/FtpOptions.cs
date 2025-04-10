namespace Basisregisters.IntegrationDb.Bosa.Infrastructure.Options
{
    public class FtpOptions
    {
        public required string Host { get; set; }
        public int? Port { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
}
