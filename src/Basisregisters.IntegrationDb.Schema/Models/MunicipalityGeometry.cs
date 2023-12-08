namespace Basisregisters.IntegrationDb.Schema.Models
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using NetTopologySuite.Geometries;

    public class MunicipalityGeometry
    {
        public int NisCode { get; set; }
        public Geometry Geometry { get; set; }

        public MunicipalityGeometry()
        { }
    }

    public sealed class MunicipalityGeometryConfiguration : IEntityTypeConfiguration<MunicipalityGeometry>
    {
        public void Configure(EntityTypeBuilder<MunicipalityGeometry> builder)
        {
            builder
                .ToTable("MunicipalityGeometries", IntegrationContext.Schema)
                .HasKey(x => x.NisCode);

            builder.Property(x => x.NisCode)
                .ValueGeneratedNever();

            builder.HasIndex(x => x.NisCode);
            builder.HasIndex(x => x.Geometry).HasMethod("GIST");
        }
    }
}
