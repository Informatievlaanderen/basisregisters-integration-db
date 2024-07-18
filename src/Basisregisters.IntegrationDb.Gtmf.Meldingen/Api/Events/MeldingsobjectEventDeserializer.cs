namespace Basisregisters.IntegrationDb.Gtmf.Meldingen.Api.Events
{
    using System;
    using Exceptions;
    using Newtonsoft.Json;

    public interface IMeldingsobjectEventDeserializer
    {
        MeldingsobjectEvent DeserializeFrom(MeldingEvent meldingEvent);
    }

    public class MeldingsobjectEventDeserializer : IMeldingsobjectEventDeserializer
    {
        public MeldingsobjectEvent DeserializeFrom(MeldingEvent meldingEvent)
        {
            try
            {
                var meldingsobjectEvent = JsonConvert.DeserializeObject<MeldingsobjectEvent>(meldingEvent.JsonData)!;
                meldingsobjectEvent.EventType = meldingEvent.EventType;

                return meldingsobjectEvent;
            }
            catch (Exception exception)
            {
                throw new MeldingsobjectDeserializationFailedException(
                    meldingEvent.Position,
                    meldingEvent.JsonData,
                    exception);
            }
        }
    }
}
