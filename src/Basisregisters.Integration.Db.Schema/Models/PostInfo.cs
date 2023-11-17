namespace Basisregisters.Integration.Db.Schema.Models
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class PostInfo
    {
        public string PostalCode { get; set; }
        public int NisCode { get; set; }
        public string Status { get; set; }

        public string? PostalNameDutch { get; set; }
        public string? PostalNameFrench { get; set; }
        public string? PostalNameGerman { get; set; }

        public string PuriId { get; set; }
        public string Namespace { get; set; }
        public string VerionString { get; set; }
        public DateTimeOffset VersionTimestamp { get; set; }

        public PostInfo()
        { }
    }

    public sealed class PostalConfiguration : IEntityTypeConfiguration<PostInfo>
    {
        public void Configure(EntityTypeBuilder<PostInfo> builder)
        {
            builder
                .ToTable("PostInfo", IntegrationContext.Schema)
                .HasKey(x => x.PostalCode);

            builder.HasIndex(x => x.NisCode);
            builder.HasIndex(x => x.Status);
            builder.HasIndex(x => x.PostalNameDutch);
            builder.HasIndex(x => x.PostalNameFrench);
            builder.HasIndex(x => x.PostalNameGerman);
            builder.HasIndex(x => x.VersionTimestamp);
        }
    }
}
