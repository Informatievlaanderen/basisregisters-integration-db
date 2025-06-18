namespace Basisregisters.Integration.Veka.Configuration
{
    public class EmailOptions
    {
        public required string SenderEmail { get; set; }
        public required bool Enabled { get; set; } = true;
    }
}
