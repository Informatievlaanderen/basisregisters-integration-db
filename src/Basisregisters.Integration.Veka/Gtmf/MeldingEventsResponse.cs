namespace Basisregisters.Integration.Veka.Gtmf
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class MeldingEventsResponse
    {
        [JsonProperty("member")] public required IList<MeldingEventResponse> Events { get; set; }
    }

    public class MeldingEventResponse
    {
        [JsonProperty("id")] public required int Position { get; set; }
        [JsonProperty("subject")] public required string MeldingId { get; set; }
        [JsonProperty("type")] public required string Type { get; set; }
    }
}
