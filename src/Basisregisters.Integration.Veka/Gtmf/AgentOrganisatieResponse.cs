namespace Basisregisters.Integration.Veka.Gtmf
{
    using Newtonsoft.Json;

    public class AgentOrganisatieResponse
    {
        [JsonProperty("wettelijkeNaam")] public required string Naam { get; set; }
    }
}
