namespace Basisregisters.IntegrationDb.Schema.Models.Views
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class ActiveStreetnameWithoutLinkedRoadSegments
    {
        public int StreetNamePersistentLocalId { get; set; }
        public int NisCode { get; set; }
        public DateTime Timestamp { get; set; }


        public ActiveStreetnameWithoutLinkedRoadSegments() { }
    }

    public sealed class ActiveStreetnameWithoutLinkedRoadSegmentsConfiguration : IEntityTypeConfiguration<ActiveStreetnameWithoutLinkedRoadSegments>
    {
        public void Configure(EntityTypeBuilder<ActiveStreetnameWithoutLinkedRoadSegments> builder)
        {
            builder
                .ToView(nameof(ActiveStreetnameWithoutLinkedRoadSegments), IntegrationContext.Schema)
                .HasNoKey()
                .ToSqlQuery(@$"
                            SELECT
                                ""StreetNamePersistentLocalId"",
                                ""NisCode"",
                                ""Timestamp""
                            FROM ""{IntegrationContext.Schema}"".""{nameof(ViewQueries.VIEW_ActiveStreetnameWithoutLinkedRoadSegments)}"" ");

            builder.HasIndex(x => x.StreetNamePersistentLocalId);
            builder.HasIndex(x => x.NisCode);
        }
    }
}
