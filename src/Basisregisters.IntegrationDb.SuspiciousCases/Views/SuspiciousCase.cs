namespace Basisregisters.IntegrationDb.SuspiciousCases.Views
{
    public abstract class SuspiciousCase
    {
        public string PersistentLocalId { get; set; }
        public string NisCode { get; set; }
        public abstract Category Category { get; }
        public string Description { get; set; }

        public SuspiciousCase() { }
    }
}
