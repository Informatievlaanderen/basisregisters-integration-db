namespace Basisregisters.Integration.Veka.Gtmf
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;
    using NodaTime;
    using NodaTime.Text;

    public class MeldingV1Response
    {
        [JsonProperty("referentie")] public string Referentie { get; set; }
        [JsonProperty("heeftParticiperende")] public IEnumerable<Partipatie> Participaties { get; set; }
        [JsonProperty("heeftDoelwit")] public IEnumerable<MeldingsObject> MeldingsObjecten { get; set; }

        public string GetIndienerId()
        {
            var indienerParticipatie = Participaties.Single(x => x.Rol.Label == "Indiener");

            return indienerParticipatie.Agent.Id.Split('/', StringSplitOptions.RemoveEmptyEntries).Last();
        }

        public string GetBeschrijving()
        {
            return GetMeldingsObject().Body.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.Beschrijving))?.Beschrijving ?? string.Empty;
        }

        public string GetIndieningsdatum()
        {
            return MeldingsObjecten.First().Toestand.DatumVaststelling;
        }

        public string GetStatus()
        {
            var laatsteStatusUpdate = GetLaatsteStatusUpdate();
            return laatsteStatusUpdate.Status.Label;
        }

        public string GetBehandelaarUri()
        {
            var laatsteStatusUpdate = GetLaatsteStatusUpdate();
            return laatsteStatusUpdate.BehandelaarId;
        }

        public string GetToelichtingBehandelaar()
        {
            var laatsteStatusUpdate = GetLaatsteStatusUpdate();
            return laatsteStatusUpdate.Toelichting;
        }

        private StatusUpdate GetLaatsteStatusUpdate()
        {
            var eindStatus = new[] { "Afgewezen", "Opgelost", "Gesloten" };
            return GetMeldingsObject().StatusUpdates.First(x => eindStatus.Contains(x.Status.Label));
        }

        private MeldingsObject GetMeldingsObject()
        {
            // There's only one meldingsobject per GRAR melding.
            return MeldingsObjecten.First();
        }
    }

    public class StatusUpdate
    {
        [JsonProperty("statuswijzigingDoor")] public string BehandelaarId { get; set; }

        [JsonProperty("toelichtingStatuswijzigingMelder")] public string Toelichting { get; set; }

        [JsonProperty("statusType")] public Status Status { get; set; }
    }

    public class Status
    {
        [JsonProperty("prefLabel")] public string Label { get; set; }
    }

    public class Toestand
    {
        [JsonProperty("datumVaststelling")] public string DatumVaststelling { get; set; }
    }

    public class MeldingsObject
    {
        [JsonProperty("gerelateerdeBody")] public IEnumerable<MeldingsObjectBody> Body { get; set; }
        [JsonProperty("heeftToestand")] public Toestand Toestand { get; set; }
        [JsonProperty("heeftStatus")] public IEnumerable<StatusUpdate> StatusUpdates { get; set; }
    }

    public class MeldingsObjectBody
    {
        [JsonProperty("beschrijving")] public string Beschrijving { get; set; }
    }

    public class Partipatie
    {
        [JsonProperty("agent")] public Agent Agent { get; set; }
        [JsonProperty("rol")] public AgentRol Rol { get; set; }
    }

    public class Agent
    {
        [JsonProperty("@id")] public string Id { get; set; }
    }

    public class AgentRol
    {
        [JsonProperty("prefLabel")] public string Label { get; set; }
    }
}
