namespace Basisregisters.IntegrationDb.Meldingen
{
    public class ProjectionState
    {
        public string Name { get; set; }
        public int Position { get; set; }

        public ProjectionState()
        { }

        public ProjectionState(string name, int position)
        {
            Name = name;
            Position = position;
        }
    }
}
