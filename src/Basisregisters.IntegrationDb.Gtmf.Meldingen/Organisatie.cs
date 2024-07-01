namespace Basisregisters.IntegrationDb.Gtmf.Meldingen
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class Organisatie
    {
        public Guid Id { get; set; }
        // Provided the Organisatie with an own Id besides the GTMF Id. Can be useful in some cases: fusies, clean up GTMF organizations (Geosecure), ...
        public Guid IdInternal { get; set; }
        public string Naam { get; set; }
        public string? OvoCode { get; set; }
        public string? KboNummer { get; set; }

        private Organisatie()
        { }

        public Organisatie(
            Guid id,
            Guid idInternal,
            string naam,
            string? ovoCode,
            string? kboNummer)
        {
            IdInternal = idInternal;
            Id = id;
            Naam = naam;
            OvoCode = ovoCode;
            KboNummer = kboNummer;
        }
    }

    public sealed class OrganisatieConfiguration : IEntityTypeConfiguration<Organisatie>
    {
        private const string TableName = "organisatie";

        public void Configure(EntityTypeBuilder<Organisatie> builder)
        {
            builder.ToTable(TableName, MeldingenContext.Schema)
                .HasKey(x => x.Id);
            // .IsClustered()

            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.IdInternal).HasColumnName("id_internal");
            builder.Property(x => x.Naam).HasColumnName("naam");
            builder.Property(x => x.OvoCode).HasColumnName("ovo_code");
            builder.Property(x => x.KboNummer).HasColumnName("kbo_nummer");

            builder.HasIndex(x => x.IdInternal);
            builder.HasIndex(x => x.OvoCode);
            builder.HasIndex(x => x.KboNummer);
        }
    }
}
