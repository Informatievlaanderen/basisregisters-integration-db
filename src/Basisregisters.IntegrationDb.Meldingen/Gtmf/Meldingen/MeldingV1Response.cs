namespace Basisregisters.IntegrationDb.Meldingen.Gtmf.Meldingen
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;

    public class MeldingV1Response
    {
        [JsonProperty("heeftDoelwit")] public IEnumerable<MeldingV1ResponseMeldingsObject> MeldingsObjecten { get; set; }

        public Guid GetMeldingsobjectId() => Guid.Parse(GetMeldingsobject()
            .Id
            .Split('/', StringSplitOptions.RemoveEmptyEntries)
            .Last());

        public string? GetOnderwerp()
        {
            return GetMeldingsobject().Onderwerp;
        }

        public string? GetBeschrijving()
        {
            return GetMeldingsobject().Body.FirstOrDefault(x =>
                !string.IsNullOrWhiteSpace(x.Beschrijving))?.Beschrijving;
        }

        public string? GetGeometrie()
        {
            return GetMeldingsobject().Body.FirstOrDefault(x =>
                !string.IsNullOrWhiteSpace(x.Geometrie))?.Geometrie;
        }

        public DateTimeOffset GetDatumVaststelling()
        {
            return GetMeldingsobject().Toestand.DatumVaststelling;
        }

        public string GetThema()
        {
            return GetMeldingsobject().Eigenschappen.Single(x => x.IsThema).Waarde;
        }

        public string GetOorzaak()
        {
            return GetMeldingsobject().Eigenschappen.Single(x => x.IsOorzaak).Waarde;
        }

        public string GetOvoCode()
        {
            return GetMeldingsobject().Eigenschappen.Single(x => x.IsOvoCode).Waarde;
        }

        private MeldingV1ResponseMeldingsObject GetMeldingsobject()
        {
            // There's only one meldingsobject per GRAR melding.
            return MeldingsObjecten.Single();
        }
    }

    public class MeldingV1ResponseToestand
    {
        [JsonProperty("datumVaststelling")] public DateTimeOffset DatumVaststelling { get; set; }
    }

    public class MeldingV1ResponseMeldingsObject
    {
        [JsonProperty("@id")] public string Id { get; set; }
        [JsonProperty("heeftOnderwerp")] public string? Onderwerp { get; set; }
        [JsonProperty("gerelateerdeBody")] public IEnumerable<MeldingV1ResponseMeldingsObjectBody> Body { get; set; }
        [JsonProperty("heeftToestand")] public MeldingV1ResponseToestand Toestand { get; set; }
        [JsonProperty("heeftEigenschap")] public IEnumerable<MeldingV1ResponseEigenschap> Eigenschappen { get; set; }
    }

    public class MeldingV1ResponseMeldingsObjectBody
    {
        [JsonProperty("beschrijving")] public string? Beschrijving { get; set; }
        [JsonProperty("geometrie")] public string? Geometrie { get; set; }
    }

    public class MeldingV1ResponseEigenschap
    {
        public const string GRAR_OvoCode = "GRAR_OvoCode";
        public const string GRAR_Thema = "GRAR_Thema";
        public const string GRAR_Oorzaak = "GRAR_Oorzaak";

        [JsonProperty("eigenschap")] public string Type { get; set; }
        [JsonProperty("voorgesteldeWaarde")] public string Waarde { get; set; }

        public bool IsOvoCode => Type.EndsWith(GRAR_OvoCode, StringComparison.InvariantCultureIgnoreCase);
        public bool IsThema => Type.EndsWith(GRAR_Thema, StringComparison.InvariantCultureIgnoreCase);
        public bool IsOorzaak => Type.EndsWith(GRAR_Oorzaak, StringComparison.InvariantCultureIgnoreCase);
    }
}
