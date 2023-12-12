namespace Basisregisters.IntegrationDb.Schema.Models.Views.SuspiciousCases
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class CurrentStreetNameWithoutLinkedRoadSegments
    {
        public int StreetNamePersistentLocalId { get; set; }
        public int NisCode { get; set; }
        public DateTimeOffset Timestamp { get; set; }


        public CurrentStreetNameWithoutLinkedRoadSegments() { }
    }

    public sealed class CurrentStreetNameWithoutLinkedRoadSegmentsConfiguration : IEntityTypeConfiguration<CurrentStreetNameWithoutLinkedRoadSegments>
    {
        public void Configure(EntityTypeBuilder<CurrentStreetNameWithoutLinkedRoadSegments> builder)
        {
            builder
                .ToView(nameof(CurrentStreetNameWithoutLinkedRoadSegments), IntegrationContext.Schema)
                .HasNoKey()
                .ToSqlQuery(@$"
                            SELECT
                                ""StreetNamePersistentLocalId"",
                                ""NisCode"",
                                ""Timestamp""
                            FROM  {Views.CurrentStreetNameWithoutLinkedRoadSegments.Table} ");

            builder.HasIndex(x => x.StreetNamePersistentLocalId);
            builder.HasIndex(x => x.NisCode);
        }
    }
}
