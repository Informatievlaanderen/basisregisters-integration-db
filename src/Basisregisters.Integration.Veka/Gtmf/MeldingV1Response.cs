namespace Basisregisters.Integration.Veka.Gtmf
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;

    public class MeldingV1Response
    {
        [JsonProperty("referentie")] public required string Referentie { get; set; }
        [JsonProperty("heeftParticiperende")] public required IEnumerable<Partipatie> Participaties { get; set; }
        [JsonProperty("heeftDoelwit")] public required IEnumerable<MeldingsObject> MeldingsObjecten { get; set; }

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
        [JsonProperty("statuswijzigingDoor")] public required string BehandelaarId { get; set; }

        [JsonProperty("toelichtingStatuswijzigingMelder")] public required string Toelichting { get; set; }

        [JsonProperty("statusType")] public required Status Status { get; set; }
    }

    public class Status
    {
        [JsonProperty("prefLabel")] public required string Label { get; set; }
    }

    public class Toestand
    {
        [JsonProperty("datumVaststelling")] public required string DatumVaststelling { get; set; }
    }

    public class MeldingsObject
    {
        [JsonProperty("gerelateerdeBody")] public required IEnumerable<MeldingsObjectBody> Body { get; set; }
        [JsonProperty("heeftToestand")] public required Toestand Toestand { get; set; }
        [JsonProperty("heeftStatus")] public required IEnumerable<StatusUpdate> StatusUpdates { get; set; }
    }

    public class MeldingsObjectBody
    {
        [JsonProperty("beschrijving")] public required string Beschrijving { get; set; }
    }

    public class Partipatie
    {
        [JsonProperty("agent")] public required Agent Agent { get; set; }
        [JsonProperty("rol")] public required AgentRol Rol { get; set; }
    }

    public class Agent
    {
        [JsonProperty("@id")] public required string Id { get; set; }
    }

    public class AgentRol
    {
        [JsonProperty("prefLabel")] public required string Label { get; set; }
    }
}
