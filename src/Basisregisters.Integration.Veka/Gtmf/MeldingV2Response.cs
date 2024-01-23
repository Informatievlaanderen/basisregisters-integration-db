namespace Basisregisters.Integration.Veka.Gtmf
{
    using Newtonsoft.Json;

    public class MeldingV2Response
    {
        [JsonProperty("referentieMelder")] public string? ReferentieMelder { get; set; }
    }
}
