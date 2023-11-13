namespace Basisregisters.Integration.Db.Schema.Models
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public sealed class Municipality
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NisCode { get; set; }
        public string Status { get; set; }
        public string OfficialLanguages { get; set; }
        public string FacilityLanguages { get; set; }
        public string NameDutch { get; set; }
        public string NameFrench { get; set; }
        public string NameGerman { get; set; }

        public string PuriId { get; set; }
        public string Namespace { get; set; }
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

            builder.HasIndex(x => x.VersionTimestamp);
            builder.HasIndex(x => x.NameDutch);
            builder.HasIndex(x => x.NameFrench);
            builder.HasIndex(x => x.NameGerman);
            builder.HasIndex(x => x.Status);
            builder.HasIndex(x => x.IsRemoved);
        }
    }
}
