namespace Basisregisters.IntegrationDb.Schema.Models
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public sealed class Municipality
    {
        public int NisCode { get; set; }
        public string Status { get; set; }

        public bool OfficialLanguageDutch { get; set; }
        public bool OfficialLanguageFrench { get; set; }
        public bool OfficialLanguageGerman { get; set; }
        public bool OfficialLanguageEnglish { get; set; }

        public bool FacilityLanguageDutch { get; set; }
        public bool FacilityLanguageFrench { get; set; }
        public bool FacilityLanguageGerman { get; set; }
        public bool FacilityLanguageEnglish { get; set; }

        public string? NameDutch { get; set; }
        public string? NameFrench { get; set; }
        public string? NameGerman { get; set; }
        public string? NameEnglish { get; set; }

        public string PuriId { get; set; }
        public string Namespace { get; set; }
        public string VerionString { get; set; }
        public DateTimeOffset VersionTimestamp { get; set; }
        public bool IsRemoved { get; set; }

        public Municipality() { }
    }

    public sealed class MunicipalitiesConfiguration : IEntityTypeConfiguration<Municipality>
    {
        public void Configure(EntityTypeBuilder<Municipality> builder)
        {
            builder
                .ToTable("Municipalities", IntegrationContext.Schema)
                .HasKey(x => x.NisCode);

            builder.Property(x => x.NisCode)
                .ValueGeneratedNever();

            builder.HasIndex(x => x.VersionTimestamp);
            builder.HasIndex(x => x.NameDutch);
            builder.HasIndex(x => x.NameFrench);
            builder.HasIndex(x => x.NameGerman);
            builder.HasIndex(x => x.NameEnglish);
            builder.HasIndex(x => x.Status);
            builder.HasIndex(x => x.IsRemoved);
        }
    }
}
