namespace Basisregisters.IntegrationDb.Meldingen.Gtmf.Events
{
    using System;
    using System.Linq;

    public sealed class MeldingEvent
    {
        public int Position { get; }
        public string MeldingId { get; }
        public string EventType { get; }
        public string JsonData { get; }

        public MeldingEvent(int position, string meldingId, string eventType, string jsonData)
        {
            Position = position;
            MeldingId = meldingId;
            EventType = eventType;
            JsonData = jsonData.Replace(@"\""", @"""");
        }

        public bool IsMeldingIngediendEvent() =>
            EventType.Equals(MeldingEventTypes.MeldingIngediendEvent, StringComparison.InvariantCultureIgnoreCase);

        public bool IsMeldingsobjectEvent() =>
            MeldingEventTypes.MeldingsObjectEvents.Contains(EventType, StringComparer.InvariantCultureIgnoreCase);
    }
}
