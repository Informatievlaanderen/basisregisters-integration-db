namespace Basisregisters.IntegrationDb.Gtmf.Meldingen.Api.Meldingen
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class EigenschapResponse
    {
        [JsonProperty("codelijst")] public required IEnumerable<EigenschapResponseBeschrijving> Items { get; set; }
    }

    public class EigenschapResponseBeschrijving
    {
        [JsonProperty("label")] public required string Label { get; set; }
        [JsonProperty("waarde")] public required string Waarde { get; set; }
    }
}
