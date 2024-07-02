namespace Basisregisters.IntegrationDb.Gtmf.Meldingen.Api.Meldingen
{
    using System;
    using Newtonsoft.Json;

    public class MeldingV2Response
    {
        [JsonProperty("referentie")] public string Referentie { get; set; }
        [JsonProperty("referentieMelder")] public string? ReferentieMelder { get; set; }
        [JsonProperty("indieningsDatum")] public DateTimeOffset DatumIndiening { get; set; }
        [JsonProperty("meldingsapplicatie")] public string Meldingsapplicatie { get; set; }
        [JsonProperty("samenvatting")] public string? Samenvatting { get; set; }
        [JsonProperty("indiener")] public MeldingV2ResponseIndiener Indiener { get; set; }

        public IndienerOrganisatie GetIndienerOrganisatie() => Indiener.GetIndienerOrganisatie();
    }

    public class MeldingV2ResponseIndiener
    {
        [JsonProperty("agentId")] public Guid Id { get; set; }
        [JsonProperty("korteNaam")] public string? PubliekeOrganisatieNaam { get; set; }
        [JsonProperty("wettelijkeNaam")] public string? GeregistreerdeOrganisatieNaam { get; set; }
        [JsonProperty("id")] public string Identifier { get; set; }
        [JsonProperty("ovoCode")] public string? GeregistreerdeOrganisatieOvoCode { get; set; }
        [JsonProperty("type")] public string OrganisatieType { get; set; }

        private bool IsGeregistreerdeOrganisatie => OrganisatieType.Equals("GeregistreerdeOrganisatie", StringComparison.InvariantCultureIgnoreCase);

        public IndienerOrganisatie GetIndienerOrganisatie()
        {
            return IsGeregistreerdeOrganisatie
                ? IndienerOrganisatie.CreateGeregistreerdeOrganisatie(
                    Id, GeregistreerdeOrganisatieNaam!, Identifier, GeregistreerdeOrganisatieOvoCode)
                : IndienerOrganisatie.CreatePubliekeOrganisatie(
                    Id, PubliekeOrganisatieNaam!, Identifier);
        }
    }
}
