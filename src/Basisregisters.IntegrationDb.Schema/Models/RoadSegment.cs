namespace Basisregisters.IntegrationDb.Schema.Models
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using NetTopologySuite.Geometries;

    public class RoadSegment
    {
        public string Id { get; set; }
        public int? Version { get; set; }
        public string? AccessRestrictionDutchName { get; set; }
        public int? AccessRestrictionId { get; set; }
        public string? BeginRoadNodeId { get; set; }
        public string? CategoryDutchName { get; set; }
        public string? CategoryId { get; set; }
        public int? EndRoadNodeId { get; set; }
        public Geometry? GeometryAsHex { get; set; }
        public string? GeometryAsWkt { get; set; }
        public int? GeometrySrid { get; set; }
        public int? GeometryVersion { get; set; }
        public string? LeftSideMunicipalityId { get; set; }
        public string? LeftSideMunicipalityNisCode { get; set; }
        public string? LeftSideStreetName { get; set; }
        public int? LeftSideStreetNameId { get; set; }
        public string? MaintainerId { get; set; }
        public string? MaintainerName { get; set; }
        public string? MethodDutchName { get; set; }
        public int? MethodId { get; set; }
        public string? MorphologyDutchName { get; set; }
        public int? MorphologyId { get; set; }
        public string? RecordingDate { get; set; } // parse_datetime
        public string? RightSideMunicipalityId { get; set; }
        public string? RightSideMunicipalityNisCode { get; set; }
        public string? RightSideStreetName { get; set; }
        public int? RightSideStreetNameId { get; set; }
        public int? RoadSegmentVersion { get; set; }
        public string? StatusDutchName { get; set; }
        public int? StatusId { get; set; }
        public int? StreetNameCachePosition { get; set; }
        public int? TransactionId { get; set; }
        public DateTimeOffset? Timestamp { get; set; } // parse_datetime
        public string? Organization { get; set; }
        public DateTimeOffset? LastChangedTimestamp { get; set; } // parse_datetime
        public bool Removed { get; set; }

        public RoadSegment()
        { }
    }

    public sealed class RoadSegmentConfiguration : IEntityTypeConfiguration<RoadSegment>
    {
        public void Configure(EntityTypeBuilder<RoadSegment> builder)
        {
            builder
                .ToTable("RoadSegments", IntegrationContext.Schema)
                .HasKey(x => x.Id);

            builder.Property(x => x.GeometryAsHex)
                .HasComputedColumnSql(IntegrationContext.GeomFromEwkbComputedQuery, stored: true);

            builder.HasIndex(x => x.GeometryAsHex).HasMethod("GIST");
            builder.HasIndex(x => x.MorphologyId);
            builder.HasIndex(x => x.StreetNameCachePosition);
        }
    }
}
