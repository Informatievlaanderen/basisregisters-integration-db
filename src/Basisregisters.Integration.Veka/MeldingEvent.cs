namespace Basisregisters.Integration.Veka
{
    using System;

    public sealed class MeldingEvent
    {
        public int Position { get; }
        public string MeldingId { get; }
        public string Type { get; }

        public MeldingEvent(int position, string meldingId, string type)
        {
            Position = position;
            MeldingId = meldingId;
            Type = type;
        }
    }
}
