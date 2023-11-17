namespace Basisregisters.Integration.Db.Schema.Models
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using NetTopologySuite.Geometries;

    public class Address
    {
        public int PersistentLocalId { get; set; }
        public int NisCode { get; set; }
        public string? PostalCode { get; set; }
        public string StreetNamePersistentLocalId { get; set; }
        public string Status { get; set; }
        public string HouseNumber { get; set; }
        public string? BoxNumber { get; set; }

        public string? FullNameDutch { get; set; }
        public string? FullNameFrench { get; set; }
        public string? FullNameGerman { get; set; }
        public string? FullNameEnglish { get; set; }

        public string GeometryGml { get; set; }
        public Geometry Geometry { get; set; }
        public string PositionMethod { get; set; }
        public string PositionSpecification { get; set; }

        public bool IsOfficiallyAssigned { get; set; }

        public string PuriId { get; set; }
        public string Namespace { get; set; }
        public string VerionString { get; set; }
        public DateTimeOffset VersionTimestamp { get; set; }
        public bool IsRemoved { get; set; }

        public Address()
        { }
    }

    public sealed class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder
                .ToTable("Addresses", IntegrationContext.Schema)
                .HasKey(x => x.PersistentLocalId);

            builder.Property(x => x.PersistentLocalId)
                .ValueGeneratedNever();

            builder.Property(x => x.Geometry)
                .HasComputedColumnSql(IntegrationContext.GmlComputedValueQuery, stored: true);

            builder.HasIndex(x => x.StreetNamePersistentLocalId);
            builder.HasIndex(x => x.PostalCode);
            builder.HasIndex(x => x.NisCode);
            builder.HasIndex(x => x.Status);
            builder.HasIndex(x => x.HouseNumber);
            builder.HasIndex(x => x.BoxNumber);
            builder.HasIndex(x => x.IsRemoved);
            builder.HasIndex(x => x.VersionTimestamp);
            builder.HasIndex(x => x.Geometry);
        }
    }
}
