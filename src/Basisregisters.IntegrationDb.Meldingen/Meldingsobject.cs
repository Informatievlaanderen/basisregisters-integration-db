namespace Basisregisters.IntegrationDb.Meldingen
{
    using System;

    public class Meldingsobject
    {
        public Guid MeldingsobjectId { get; set; }
        public Guid MeldingId { get; set; }
        public DateTimeOffset DatumIndiening { get; set; }
        public DateTimeOffset DatumVaststelling { get; set; }
        public Guid MeldingsOrganisatieId { get; set; }
        public Guid GtmfMeldingsOrganisatieId { get; set; }
        public string Meldingsapplicatie { get; set; }
        public string Referentie { get; set; }
        public string? ReferentieMelder { get; set; }
        public string? Onderwerp { get; set; }
        public string? Beschrijving { get; set; }
        public string? Samenvatting { get; set; }
        public string Thema { get; set; }
        public string Oorzaak { get; set; }
        public string OvoCode { get; set; }

        private Meldingsobject()
        { }

        public Meldingsobject(
            Guid meldingsobjectId,
            Guid meldingId,
            DateTimeOffset datumIndiening,
            DateTimeOffset datumVaststelling,
            Guid gtmfMeldingsOrganisatieId,
            string meldingsapplicatie,
            string referentie,
            string? referentieMelder,
            string? onderwerp,
            string? beschrijving,
            string? samenvatting,
            string thema,
            string oorzaak,
            string ovoCode)
        {
            MeldingsobjectId = meldingsobjectId;
            MeldingId = meldingId;
            DatumIndiening = datumIndiening;
            DatumVaststelling = datumVaststelling;
            GtmfMeldingsOrganisatieId = gtmfMeldingsOrganisatieId;
            Meldingsapplicatie = meldingsapplicatie;
            Referentie = referentie;
            ReferentieMelder = referentieMelder;
            Onderwerp = onderwerp;
            Beschrijving = beschrijving;
            Samenvatting = samenvatting;
            Thema = thema;
            Oorzaak = oorzaak;
            OvoCode = ovoCode;
        }
    }
}
