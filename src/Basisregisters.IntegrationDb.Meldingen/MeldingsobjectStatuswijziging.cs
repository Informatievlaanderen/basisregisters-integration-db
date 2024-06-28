namespace Basisregisters.IntegrationDb.Meldingen
{
    using System;

    public class MeldingsobjectStatuswijziging
    {
        public Guid MeldingsobjectId { get; set; }
        public Guid MeldingId { get; set; }
        public string? OudeStatus { get; set; }
        public string NieuweStatus { get; set; }
        public Guid OrganisatieId { get; set; }
        public DateTimeOffset TijdstipWijziging { get; set; }
        public string? Toelichting { get; set; }

        private MeldingsobjectStatuswijziging()
        { }

        public MeldingsobjectStatuswijziging(
            Guid meldingsobjectId,
            Guid meldingId,
            string? oudeStatus,
            string nieuweStatus,
            Guid organisatieId,
            DateTimeOffset tijdstipWijziging,
            string? toelichting)
        {
            MeldingsobjectId = meldingsobjectId;
            MeldingId = meldingId;
            OudeStatus = oudeStatus;
            NieuweStatus = nieuweStatus;
            OrganisatieId = organisatieId;
            TijdstipWijziging = tijdstipWijziging;
            Toelichting = toelichting;
        }
    }
}
