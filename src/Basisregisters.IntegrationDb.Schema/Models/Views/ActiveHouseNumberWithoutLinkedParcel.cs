namespace Basisregisters.IntegrationDb.Schema.Models.Views
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using NetTopologySuite.Geometries;

    public class ActiveHouseNumberWithoutLinkedParcel
    {
        public int PersistentLocalId { get; set; }
        public string NisCode { get; set; }
        public string? PostalCode { get; set; }
        public int StreetNamePersistentLocalId { get; set; }
        public string Status { get; set; }
        public string HouseNumber { get; set; }
        public string? BoxNumber { get; set; }

        public string? FullName { get; set; }

        public string GeometryGml { get; set; }
        public Geometry Geometry { get; set; }
        public string PositionMethod { get; set; }
        public string PositionSpecification { get; set; }
        public bool IsOfficiallyAssigned { get; set; }
        public bool IsRemoved { get; set; }


        public string PuriId { get; set; }
        public string Namespace { get; set; }
        public string VersionString { get; set; }
        public DateTimeOffset VersionTimestamp { get; set; }

        public ActiveHouseNumberWithoutLinkedParcel() { }
    }

    public sealed class ActiveHouseNumberWithoutLinkedParcelConfiguration : IEntityTypeConfiguration<ActiveHouseNumberWithoutLinkedParcel>
    {
        public void Configure(EntityTypeBuilder<ActiveHouseNumberWithoutLinkedParcel> builder)
        {
            builder
                .ToView(nameof(ActiveHouseNumberWithoutLinkedParcel), IntegrationContext.Schema)
                .HasNoKey()
                .ToSqlQuery(@"select
                                ""PersistentLocalId""
                                ,""NisCode""
                                ,""PostalCode""
                                ,""StreetNamePersistentLocalId""
                                ,""Status""
                                ,""HouseNumber""
                                ,""BoxNumber""
                                ,""FullName""
                                ,""GeometryGml""
                                ,""Geometry""
                                ,""PositionMethod""
                                ,""PositionSpecification""
                                ,""IsOfficiallyAssigned""
                                ,""IsRemoved""
                                ,""PuriId""
                                ,""Namespace""
                                ,""VersionString""
                                ,""VersionTimestamp""
                            FROM ""Integration"".""VIEW_ActiveHouseNumberWithoutLinkedParcel"";");
        }
    }
}
