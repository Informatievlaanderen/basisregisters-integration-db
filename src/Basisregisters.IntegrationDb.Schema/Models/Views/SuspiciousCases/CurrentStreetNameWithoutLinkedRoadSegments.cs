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


        public CurrentStreetNameWithoutLinkedRoadSegments()
        { }
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
                            FROM  {ViewName} ");
        }

        public const string ViewName = @$"""{IntegrationContext.Schema}"".""VIEW_{nameof(CurrentStreetNameWithoutLinkedRoadSegments)}""";

        public const string Create = $@"
            CREATE MATERIALIZED VIEW IF NOT EXISTS {ViewName} AS
            SELECT
                streetName.""PersistentLocalId"" AS ""StreetNamePersistentLocalId"",
                streetName.""NisCode"",
                CURRENT_TIMESTAMP AS ""Timestamp""
            FROM ""Integration"".""StreetNames"" AS streetName
            WHERE NOT EXISTS (
                SELECT 1
                FROM ""Integration"".""RoadSegments"" roadsegment
                WHERE roadsegment.""LeftSideStreetNameId"" = streetname.""PersistentLocalId""
                AND roadsegment.""RightSideStreetNameId"" = streetname.""PersistentLocalId""
            )
            AND streetName.""Status"" ILIKE 'ingebruik'
            AND streetName.""IsRemoved"" = false;

            CREATE INDEX ""IX_{nameof(CurrentStreetNameWithoutLinkedRoadSegments)}_StreetNamePersistentLocalId"" ON {ViewName} USING btree (""{nameof(CurrentStreetNameWithoutLinkedRoadSegments.StreetNamePersistentLocalId)}"");
            CREATE INDEX ""IX_{nameof(CurrentStreetNameWithoutLinkedRoadSegments)}_NisCode"" ON {ViewName} USING btree (""{nameof(CurrentStreetNameWithoutLinkedRoadSegments.NisCode)}"");
            ";
    }
}
