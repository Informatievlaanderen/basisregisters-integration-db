namespace Basisregisters.IntegrationDb.Schema.Models.Views
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class CurrentStreetnameWithoutLinkedRoadSegments
    {
        public int StreetNamePersistentLocalId { get; set; }
        public int NisCode { get; set; }
        public DateTimeOffset Timestamp { get; set; }


        public CurrentStreetnameWithoutLinkedRoadSegments() { }
    }

    public sealed class CurrentStreetnameWithoutLinkedRoadSegmentsConfiguration : IEntityTypeConfiguration<CurrentStreetnameWithoutLinkedRoadSegments>
    {
        public void Configure(EntityTypeBuilder<CurrentStreetnameWithoutLinkedRoadSegments> builder)
        {
            builder
                .ToView(nameof(CurrentStreetnameWithoutLinkedRoadSegments), IntegrationContext.Schema)
                .HasNoKey()
                .ToSqlQuery(@$"
                            SELECT
                                ""StreetNamePersistentLocalId"",
                                ""NisCode"",
                                ""Timestamp""
                            FROM  {Views.CurrentStreetnameWithoutLinkedRoadSegments.Table} ");

            builder.HasIndex(x => x.StreetNamePersistentLocalId);
            builder.HasIndex(x => x.NisCode);
        }
    }
}
