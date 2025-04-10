namespace Basisregisters.IntegrationDb.SuspiciousCases.Views
{
    public abstract class SuspiciousCase
    {
        public string PersistentLocalId { get; set; } = null!;
        public string NisCode { get; set; } = null!;
        public abstract Category Category { get; }
        public string Description { get; set; } = null!;

        protected SuspiciousCase() { }
    }
}
