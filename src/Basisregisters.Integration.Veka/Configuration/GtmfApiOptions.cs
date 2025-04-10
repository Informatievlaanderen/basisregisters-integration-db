namespace Basisregisters.Integration.Veka.Configuration
{
    public class GtmfApiOptions
    {
        public required string BaseUrl { get; set; }
        public required string TokenEndpoint { get; set; }
        public required string ClientId { get; set; }
        public required string ClientSecret { get; set; }
    }
}
