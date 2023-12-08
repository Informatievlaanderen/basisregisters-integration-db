namespace Basisregisters.IntegrationDb.Schema.Models
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class Parcel
    {
        public string CaPaKey { get; set; }
        public string? Status { get; set; }

        public string? Addresses { get; set; }
        public bool IsRemoved { get; set; }

        // public string GeometryGml { get; set; }
        // public Geometry Geometry { get; set; }


        public string? PuriId { get; set; }
        public string? Namespace { get; set; }
        public string? VersionString { get; set; }
        public DateTimeOffset? VersionTimestamp { get; set; }
        public long? IdempotenceKey { get; set; }

        public Parcel()
        { }
    }

    public sealed class ParcelConfiguration : IEntityTypeConfiguration<Parcel>
    {
        public void Configure(EntityTypeBuilder<Parcel> builder)
        {
            builder
                .ToTable("Parcels", IntegrationContext.Schema)
                .HasKey(x => x.CaPaKey);

            builder.Property(x => x.CaPaKey)
                .ValueGeneratedNever();

            // builder.Property(x => x.Geometry)
            //     .HasComputedColumnSql(IntegrationContext.GmlComputedValueQuery, stored: true);

            builder.HasIndex(x => x.CaPaKey);
            builder.HasIndex(x => x.Status);
            builder.HasIndex(x => x.IsRemoved);
            builder.HasIndex(x => x.VersionTimestamp);
        }
    }
}
