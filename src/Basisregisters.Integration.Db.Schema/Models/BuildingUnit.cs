namespace Basisregisters.Integration.Db.Schema.Models
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class BuildingUnit
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PersistentLocalId { get; set; }
        public int BuildingPersistentLocalId { get; set; }
        public string Status { get; set; }
        public string Functie { get; set; }
        public string GeometryMethod { get; set; }
        public string GeometryGML { get; set; }
        // searchable geometry field?
        public bool HasDeviation { get; set; }

        public string PuriId { get; set; }
        public string Namespace { get; set; }
        public DateTimeOffset VersionTimestamp { get; set; }
        public bool IsRemoved { get; set; }

        public BuildingUnit()
        { }
    }

    public sealed class BuildingUnitConfiguration : IEntityTypeConfiguration<BuildingUnit>
    {
        public void Configure(EntityTypeBuilder<BuildingUnit> builder)
        {
            builder
                .ToTable("BuildingUnits", IntegrationContext.Schema)
                .HasKey(x => x.PersistentLocalId);
            //
            // builder.Property(x => x.PersistentLocalId)
            //     .ValueGeneratedNever();

            builder.HasIndex(x => x.Status);
            builder.HasIndex(x => x.IsRemoved);
            builder.HasIndex(x => x.VersionTimestamp);
        }
    }
}
