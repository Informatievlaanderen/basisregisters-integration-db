namespace Basisregisters.Integration.Veka.Gtmf
{
    using Newtonsoft.Json;

    public class AgentOrganisatieResponse
    {
        [JsonProperty("wettelijkeNaam")] public string Naam { get; set; }
    }
}
