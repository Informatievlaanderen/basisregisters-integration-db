namespace Basisregisters.IntegrationDb.Meldingen.Gtmf.Events
{
    using System;
    using Newtonsoft.Json;

    public class MeldingsobjectEvent
    {
        [JsonIgnore] public string EventType { get; set; }
        [JsonProperty("MeldingsobjectId")] public Guid MeldingsobjectId { get; set; }
        [JsonProperty("MeldingId")] public Guid MeldingId { get; set; }
        [JsonProperty("ToelichtingIntern")] public string ToelichtingMelder { get; set; }
        [JsonProperty("Initiator")] public Initiator Initiator { get; set; }
        [JsonProperty("AangemaaktOp")] public DateTimeOffset AangemaaktOp { get; set; }

        public string GetOudeStatus()
        {
            return EventType switch
            {
                MeldingEventTypes.MeldingsobjectInitieelToegewezenAanBehandelaarEvent => MeldingsobjectStatussen.Ingediend,
                MeldingEventTypes.MeldingsobjectInitieelToegewezenAanBronhouderEvent => MeldingsobjectStatussen.Ingediend,
                MeldingEventTypes.MeldingsobjectGeslotenNaToewijzingAlsBronhouderEvent => MeldingsobjectStatussen.Toegewezen,
                MeldingEventTypes.MeldingsobjectInOnderzoekEvent => MeldingsobjectStatussen.Toegewezen,
                MeldingEventTypes.MeldingsobjectAfgewezenNaOnderzoekAlsBehandelaarEvent => MeldingsobjectStatussen.InOnderzoek,
                MeldingEventTypes.MeldingsobjectGeslotenNaOnderzoekAlsBehandelaarEvent => MeldingsobjectStatussen.InOnderzoek,
                MeldingEventTypes.MeldingsobjectGeslotenNaOnderzoekAlsBronhouderEvent => MeldingsobjectStatussen.InOnderzoek,
                MeldingEventTypes.MeldingsobjectOpgelostEvent => MeldingsobjectStatussen.InOnderzoek,
                _ => throw new NotImplementedException()
            };
        }

        public string GetNieuweStatus()
        {
            return EventType switch
            {
                MeldingEventTypes.MeldingsobjectInitieelToegewezenAanBehandelaarEvent => MeldingsobjectStatussen.Toegewezen,
                MeldingEventTypes.MeldingsobjectInitieelToegewezenAanBronhouderEvent => MeldingsobjectStatussen.Toegewezen,
                MeldingEventTypes.MeldingsobjectGeslotenNaToewijzingAlsBronhouderEvent => MeldingsobjectStatussen.Gesloten,
                MeldingEventTypes.MeldingsobjectInOnderzoekEvent => MeldingsobjectStatussen.InOnderzoek,
                MeldingEventTypes.MeldingsobjectAfgewezenNaOnderzoekAlsBehandelaarEvent => MeldingsobjectStatussen.Afgewezen,
                MeldingEventTypes.MeldingsobjectGeslotenNaOnderzoekAlsBehandelaarEvent => MeldingsobjectStatussen.Gesloten,
                MeldingEventTypes.MeldingsobjectGeslotenNaOnderzoekAlsBronhouderEvent => MeldingsobjectStatussen.Gesloten,
                MeldingEventTypes.MeldingsobjectOpgelostEvent => MeldingsobjectStatussen.Opgelost,
                _ => throw new NotImplementedException()
            };
        }

        public BehandelendeOrganisatie GetBehandelendeOrganisatie()
        {
            if (Initiator.Type.Equals("Applicatie", StringComparison.InvariantCultureIgnoreCase))
            {
                return new BehandelendeOrganisatie(Initiator.AgentId, "GTMF-Systeem", null, null);
            }

            return new BehandelendeOrganisatie(Initiator.AgentId, Initiator.Naam, Initiator.OvoCode, Initiator.KboNummer);
        }
    }

    public class Initiator
    {
        [JsonProperty("ovoCode")] public string OvoCode { get; set; }
        [JsonProperty("id")] public string KboNummer { get; set; }
        [JsonProperty("wettelijkeNaam")] public string Naam { get; set; }
        [JsonProperty("agentId")] public Guid AgentId { get; set; }
        [JsonProperty("type")] public string Type { get; set; }
    }
}
