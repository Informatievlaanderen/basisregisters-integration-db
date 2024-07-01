namespace Basisregisters.IntegrationDb.Meldingen.Gtmf.Meldingen
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class EigenschapResponse
    {
        [JsonProperty("codelijst")] public IEnumerable<EigenschapResponseBeschrijving> Items { get; set; }
    }

    public class EigenschapResponseBeschrijving
    {
        [JsonProperty("label")] public string Label { get; set; }
        [JsonProperty("waarde")] public string Waarde { get; set; }
    }
}
