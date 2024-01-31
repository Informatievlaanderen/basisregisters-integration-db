namespace Basisregisters.Integration.Veka
{
    using System;

    public sealed class Melding
    {
        public Guid Id { get; }
        public bool IsIngediendDoorVeka { get; }
        public string? ReferentieMelder { get; }
        public string Referentie { get; }
        public string Beschrijving { get; }
        public string Behandelaar { get; }
        public string Status { get; }
        public string ToelichtingBehandelaar { get; }
        public DateTime? DatumVaststelling { get; }

        public static Melding NietVekaMelding(string id)
            => new Melding(
                Guid.Parse(id), false, null, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, null);

        public static Melding VekaMelding(
            string id,
            string? referentieMelder,
            string referentie,
            string beschrijving,
            string behandelaar,
            string status,
            string toelichtingBehandelaar,
            string datumVaststelling)
            => new Melding(
                Guid.Parse(id),
                true,
                referentieMelder,
                referentie,
                beschrijving,
                behandelaar,
                status,
                toelichtingBehandelaar,
                DateTime.Parse(datumVaststelling));

        private Melding(Guid id,
            bool isIngediendDoorVeka,
            string? referentieMelder,
            string referentie,
            string beschrijving,
            string behandelaar,
            string status,
            string toelichtingBehandelaar,
            DateTime? datumVaststelling)
        {
            Id = id;
            IsIngediendDoorVeka = isIngediendDoorVeka;
            ReferentieMelder = referentieMelder;
            Referentie = referentie;
            Beschrijving = beschrijving;
            Behandelaar = behandelaar;
            Status = status;
            ToelichtingBehandelaar = toelichtingBehandelaar;
            DatumVaststelling = datumVaststelling;
        }
    }
}
