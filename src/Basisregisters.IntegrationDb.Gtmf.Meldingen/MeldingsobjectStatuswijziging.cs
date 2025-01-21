namespace Basisregisters.IntegrationDb.Gtmf.Meldingen
{
    using System;
    using Be.Vlaanderen.Basisregisters.Utilities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using NodaTime;

    public class MeldingsobjectStatuswijziging
    {
        public const string TijdstipWijzigingBackingPropertyName = nameof(TijdstipWijzigingAsDateTimeOffset);

        public Guid MeldingsobjectId { get; set; }
        public Guid MeldingId { get; set; }
        public string? OudeStatus { get; set; }
        public string NieuweStatus { get; set; }
        public Guid InitiatorOrganisatieIdInternal { get; set; }
        public string TijdstipWijzigingAsString { get; set; }
        private DateTimeOffset TijdstipWijzigingAsDateTimeOffset { get; set; }

        public Instant TijdstipWijzigingTimestamp
        {
            get => Instant.FromDateTimeOffset(TijdstipWijzigingAsDateTimeOffset);
            set
            {
                TijdstipWijzigingAsDateTimeOffset = value.ToDateTimeOffset();
                TijdstipWijzigingAsString = new Rfc3339SerializableDateTimeOffset(value.ToBelgianDateTimeOffset()).ToString();
            }
        }
        public string? Toelichting { get; set; }

        private MeldingsobjectStatuswijziging()
        { }

        public MeldingsobjectStatuswijziging(
            Guid meldingsobjectId,
            Guid meldingId,
            string? oudeStatus,
            string nieuweStatus,
            Guid initiatorOrganisatieIdInternal,
            Instant tijdstipWijziging,
            string? toelichting)
        {
            MeldingsobjectId = meldingsobjectId;
            MeldingId = meldingId;
            OudeStatus = oudeStatus;
            NieuweStatus = nieuweStatus;
            InitiatorOrganisatieIdInternal = initiatorOrganisatieIdInternal;
            TijdstipWijzigingTimestamp = tijdstipWijziging;
            Toelichting = toelichting;
        }
    }

    public sealed class MeldingsobjectStatuswijzigingConfiguration : IEntityTypeConfiguration<MeldingsobjectStatuswijziging>
    {
        private const string TableName = "meldingsobject_statuswijziging";

        public void Configure(EntityTypeBuilder<MeldingsobjectStatuswijziging> builder)
        {
            builder.ToTable(TableName, MeldingenContext.Schema)
                .HasKey(x => new { x.MeldingsobjectId, x.NieuweStatus });
            // .IsClustered()

            builder.Property(x => x.MeldingsobjectId).HasColumnName("meldingsobject_id");
            builder.Property(x => x.MeldingId).HasColumnName("melding_id");
            builder.Property(x => x.OudeStatus).HasColumnName("oude_status");
            builder.Property(x => x.NieuweStatus).HasColumnName("nieuwe_status");
            builder.Property(x => x.InitiatorOrganisatieIdInternal).HasColumnName("initiator_organisatie_id_internal");
            builder.Property(x => x.TijdstipWijzigingAsString).HasColumnName("tijdstip_wijziging_as_string");
            builder.Property(MeldingsobjectStatuswijziging.TijdstipWijzigingBackingPropertyName).HasColumnName("tijdstip_wijziging");
            builder.Property(x => x.Toelichting).HasColumnName("toelichting");

            builder.Ignore(x => x.TijdstipWijzigingTimestamp);

            builder.HasIndex(x => x.MeldingId);
            builder.HasIndex(x => x.OudeStatus);
            builder.HasIndex(x => x.NieuweStatus);
            builder.HasIndex(x => x.InitiatorOrganisatieIdInternal);
        }
    }
}
