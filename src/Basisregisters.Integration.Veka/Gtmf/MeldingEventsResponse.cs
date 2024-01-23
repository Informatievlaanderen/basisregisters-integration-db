namespace Basisregisters.Integration.Veka.Gtmf
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class MeldingEventsResponse
    {
        [JsonProperty("member")] public IList<MeldingEventResponse> Events { get; set; }
    }

    public class MeldingEventResponse
    {
        [JsonProperty("id")] public int Position { get; set; }
        [JsonProperty("subject")] public string MeldingId { get; set; }
        [JsonProperty("type")] public string Type { get; set; }
    }
}
