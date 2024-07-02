namespace Basisregisters.IntegrationDb.Gtmf.Meldingen
{
    using System;
    using Be.Vlaanderen.Basisregisters.Utilities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using NetTopologySuite.Geometries;
    using NodaTime;

    public class Meldingsobject
    {
        public const string DatumIndieningBackingPropertyName = nameof(DatumIndieningAsDateTimeOffset);
        public const string DatumVaststellingBackingPropertyName = nameof(DatumVaststellingAsDateTimeOffset);

        public Guid MeldingsobjectId { get; set; }
        public Guid MeldingId { get; set; }

        public string DatumIndieningAsString { get; set; }
        private DateTimeOffset DatumIndieningAsDateTimeOffset { get; set; }
        public Instant DatumIndieningTimestamp
        {
            get => Instant.FromDateTimeOffset(DatumIndieningAsDateTimeOffset);
            set
            {
                DatumIndieningAsDateTimeOffset = value.ToDateTimeOffset();
                DatumIndieningAsString = new Rfc3339SerializableDateTimeOffset(value.ToBelgianDateTimeOffset()).ToString();
            }
        }

        public string DatumVaststellingAsString { get; set; }
        private DateTimeOffset DatumVaststellingAsDateTimeOffset { get; set; }
        public Instant DatumVaststellingTimestamp
        {
            get => Instant.FromDateTimeOffset(DatumVaststellingAsDateTimeOffset);
            set
            {
                DatumVaststellingAsDateTimeOffset = value.ToDateTimeOffset();
                DatumVaststellingAsString = new Rfc3339SerializableDateTimeOffset(value.ToBelgianDateTimeOffset()).ToString();
            }
        }


        public Guid MeldingsorganisatieIdInternal { get; set; }
        public Guid MeldingsorganisatieId { get; set; }
        public string Meldingsapplicatie { get; set; }
        public string Referentie { get; set; }
        public string? ReferentieMelder { get; set; }
        public string? Onderwerp { get; set; }
        public string? Beschrijving { get; set; }
        public string? Samenvatting { get; set; }
        public string Thema { get; set; }
        public string Oorzaak { get; set; }
        public string OvoCode { get; set; }
        public Geometry? Geometrie { get; set; }

        private Meldingsobject()
        { }

        public Meldingsobject(
            Guid meldingsobjectId,
            Guid meldingId,
            Instant datumIndiening,
            Instant datumVaststelling,
            Guid meldingsorganisatieId,
            string meldingsapplicatie,
            string referentie,
            string? referentieMelder,
            string? onderwerp,
            string? beschrijving,
            string? samenvatting,
            string thema,
            string oorzaak,
            string ovoCode,
            Geometry? geometrie)
        {
            MeldingsobjectId = meldingsobjectId;
            MeldingId = meldingId;
            DatumIndieningTimestamp = datumIndiening;
            DatumVaststellingTimestamp = datumVaststelling;
            MeldingsorganisatieId = meldingsorganisatieId;
            Meldingsapplicatie = meldingsapplicatie;
            Referentie = referentie;
            ReferentieMelder = referentieMelder;
            Onderwerp = onderwerp;
            Beschrijving = beschrijving;
            Samenvatting = samenvatting;
            Thema = thema;
            Oorzaak = oorzaak;
            OvoCode = ovoCode;
            Geometrie = geometrie;
        }
    }

    public sealed class MeldingsobjectConfiguration : IEntityTypeConfiguration<Meldingsobject>
    {
        private const string TableName = "meldingsobject";

        public void Configure(EntityTypeBuilder<Meldingsobject> builder)
        {
            builder.ToTable(TableName, MeldingenContext.Schema)
                .HasKey(x => x.MeldingsobjectId);
                // .IsClustered()

        builder.Property(x => x.MeldingsobjectId).HasColumnName("meldingsobject_id");
        builder.Property(x => x.MeldingId).HasColumnName("melding_id");
        builder.Property(x => x.DatumIndieningAsString).HasColumnName("datum_indiening_as_string");
        builder.Property(Meldingsobject.DatumIndieningBackingPropertyName).HasColumnName("datum_indiening");
        builder.Property(x => x.DatumVaststellingAsString).HasColumnName("datum_vaststelling_as_string");
        builder.Property(Meldingsobject.DatumVaststellingBackingPropertyName).HasColumnName("datum_vaststelling");
        builder.Property(x => x.MeldingsorganisatieIdInternal).HasColumnName("meldingsorganisatie_id_internal");
        builder.Property(x => x.MeldingsorganisatieId).HasColumnName("meldingsorganisatie_id");
        builder.Property(x => x.Meldingsapplicatie).HasColumnName("meldingsapplicatie");
        builder.Property(x => x.Referentie).HasColumnName("referentie");
        builder.Property(x => x.ReferentieMelder).HasColumnName("referentie_melder");
        builder.Property(x => x.Onderwerp).HasColumnName("onderwerp");
        builder.Property(x => x.Beschrijving).HasColumnName("beschrijving");
        builder.Property(x => x.Samenvatting).HasColumnName("samenvatting");
        builder.Property(x => x.Thema).HasColumnName("thema");
        builder.Property(x => x.Oorzaak).HasColumnName("oorzaak");
        builder.Property(x => x.OvoCode).HasColumnName("ovo_code");
        builder.Property(x => x.Geometrie).HasColumnName("geometrie");

        builder.Property(p => p.Geometrie).HasColumnType("geometry");

        builder.Ignore(x => x.DatumIndieningTimestamp);
        builder.Ignore(x => x.DatumVaststellingTimestamp);

        builder.HasIndex(x => x.MeldingId);
        builder.HasIndex(x => x.MeldingsorganisatieId);
        builder.HasIndex(x => x.MeldingsorganisatieIdInternal);
        builder.HasIndex(x => x.Meldingsapplicatie);
        builder.HasIndex(x => x.Referentie);
        builder.HasIndex(x => x.ReferentieMelder);
        builder.HasIndex(x => x.Onderwerp);
        builder.HasIndex(x => x.Thema);
        builder.HasIndex(x => x.Oorzaak);
        builder.HasIndex(x => x.OvoCode);
        }
    }

    public static class NodaHelpers
    {
        private static readonly DateTimeZone? BelgianDateTimeZone = DateTimeZoneProviders.Tzdb.GetZoneOrNull("Europe/Brussels");

        public static DateTimeOffset ToBelgianDateTimeOffset(this Instant value)
            => value.InZone(BelgianDateTimeZone ?? throw new InvalidOperationException($"BelgianDateTimeZone is null")).ToDateTimeOffset();
    }
}
