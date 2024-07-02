namespace Basisregisters.IntegrationDb.Gtmf.Meldingen.Api.Events
{
    using Newtonsoft.Json;

    public interface IMeldingsobjectEventDeserializer
    {
        MeldingsobjectEvent DeserializeFrom(MeldingEvent meldingEvent);
    }

    public class MeldingsobjectEventDeserializer : IMeldingsobjectEventDeserializer
    {
        public MeldingsobjectEvent DeserializeFrom(MeldingEvent meldingEvent)
        {
            var meldingsobjectEvent = JsonConvert.DeserializeObject<MeldingsobjectEvent>(meldingEvent.JsonData)!;
            meldingsobjectEvent.EventType = meldingEvent.EventType;

            return meldingsobjectEvent;
        }
    }
}
