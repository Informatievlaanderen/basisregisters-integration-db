namespace Basisregisters.IntegrationDb.Schema.Models
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public sealed class StreetName
    {
        public int PersistentLocalId { get; set; }
        public int? NisCode { get; set; }
        public string? Status { get; set; }

        public string? NameDutch { get; set; }
        public string? NameFrench { get; set; }
        public string? NameGerman { get; set; }
        public string? NameEnglish { get; set; }

        public string? HomonymAdditionDutch { get; set; }
        public string? HomonymAdditionFrench { get; set; }
        public string? HomonymAdditionGerman { get; set; }
        public string? HomonymAdditionEnglish { get; set; }
        public bool IsRemoved { get; set; }


        public string? PuriId { get; set; }
        public string? Namespace { get; set; }
        public string? VersionString { get; set; }
        public DateTimeOffset? VersionTimestamp { get; set; }
        public long? IdempotenceKey { get; set; }

        public StreetName() { }
    }

    public sealed class StreetNamesConfiguration : IEntityTypeConfiguration<StreetName>
    {
        public void Configure(EntityTypeBuilder<StreetName> builder)
        {
            builder
                .ToTable("StreetNames", IntegrationContext.Schema)
                .HasKey(x => x.PersistentLocalId);

             builder.Property(x => x.PersistentLocalId)
                 .ValueGeneratedNever();

            builder.HasIndex(x => x.PersistentLocalId);
            builder.HasIndex(x => x.Status);
            builder.HasIndex(x => x.NisCode);
            builder.HasIndex(x => x.IsRemoved);

            builder.HasIndex(x => x.NameDutch);
            builder.HasIndex(x => x.NameFrench);
            builder.HasIndex(x => x.NameGerman);
            builder.HasIndex(x => x.NameEnglish);

            builder.HasIndex(x => x.VersionTimestamp);
        }
    }
}
