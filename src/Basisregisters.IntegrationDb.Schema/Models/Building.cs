﻿namespace Basisregisters.IntegrationDb.Schema.Models
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using NetTopologySuite.Geometries;

    public class Building
    {
        public int PersistentLocalId { get; set; }
        public string? Status { get; set; }
        public string? GeometryMethod { get; set; }
        public string? GeometryGml { get; set; }
        public Geometry? Geometry { get; set; }
        public bool IsRemoved { get; set; }


        public string? PuriId { get; set; }
        public string? Namespace { get; set; }
        public string? VersionString { get; set; }
        public DateTimeOffset? VersionTimestamp { get; set; }
        public long? IdempotenceKey { get; set; }

        public Building()
        { }
    }

    public sealed class BuildingConfiguration : IEntityTypeConfiguration<Building>
    {
        public void Configure(EntityTypeBuilder<Building> builder)
        {
            builder
                .ToTable("Buildings", IntegrationContext.Schema)
                .HasKey(x => x.PersistentLocalId);

            builder.Property(x => x.PersistentLocalId)
                .ValueGeneratedNever();

            builder.Property(x => x.Geometry)
                .HasComputedColumnSql(IntegrationContext.GeomFromGmlComputedQuery, stored: true);

            builder.HasIndex(x => x.PersistentLocalId);
            builder.HasIndex(x => x.Status);
            builder.HasIndex(x => x.IsRemoved);
            builder.HasIndex(x => x.VersionTimestamp);
            builder.HasIndex(x => x.Geometry).HasMethod("GIST");
        }
    }
}
