namespace Basisregisters.IntegrationDb.SuspiciousCases.Views
{
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class SuspiciousCaseCount
    {
        public string NisCode { get; set; }
        public int Count { get; set; }
        public SuspiciousCasesType Type { get; set; }

        public SuspiciousCaseCount()
        { }
    }

    public sealed class SuspiciousCaseCountConfiguration : IEntityTypeConfiguration<SuspiciousCaseCount>
    {
        public void Configure(EntityTypeBuilder<SuspiciousCaseCount> builder)
        {
            builder
                .ToView(ViewName, Schema.SuspiciousCases)
                .HasNoKey()
                .ToSqlQuery(@$"select
                                nis_code,
                                count,
                                type
                            FROM {Schema.SuspiciousCases}.{ViewName}");

            builder.Property(x => x.NisCode)
                .HasColumnName("nis_code");
            builder.Property(x => x.Count)
                .HasColumnName("count");
            builder.Property(x => x.Type)
                .HasColumnName("type");
        }

        public const string ViewName = "view_suspicious_cases_counts";

        public static readonly string Refresh = $@"
DO $$
BEGIN
    -- Check if the materialized view exists
    IF EXISTS (SELECT 1 FROM pg_matviews WHERE schemaname = '{Schema.SuspiciousCases}' AND matviewname = '{ViewName}') THEN
        -- Refresh the materialized view concurrently if it exists
        REFRESH MATERIALIZED VIEW CONCURRENTLY {Schema.SuspiciousCases}.{ViewName};
    END IF;
END $$;
";

        public static readonly string Create =
            $$"""
              DROP MATERIALIZED VIEW IF EXISTS {{Schema.SuspiciousCases}}.{{ViewName}};

              CREATE MATERIALIZED VIEW {{Schema.SuspiciousCases}}.{{ViewName}}
              AS
              {{CreateScript(SuspiciousCasesType.ActiveAddressLinkedToMultipleBuildingUnits, ActiveAddressLinkedToMultipleBuildingUnitsConfiguration.ViewName)}}
              UNION
              {{CreateScript(SuspiciousCasesType.ActiveAddressOutsideOfMunicipalityBounds, ActiveAddressOutsideMunicipalityBoundsConfiguration.ViewName)}}
              UNION
              {{CreateScript(SuspiciousCasesType.AddressLongerThanTwoYearsProposed, AddressLongerThanTwoYearsProposedConfiguration.ViewName)}}
              UNION
              {{CreateScript(SuspiciousCasesType.BuildingLongerThanTwoYearsPlanned, BuildingLongerThanTwoYearsPlannedConfiguration.ViewName)}}
              UNION
              {{CreateScript(SuspiciousCasesType.BuildingUnitLongerThanTwoYearsPlanned, BuildingUnitsLongerThanTwoYearsPlannedConfiguration.ViewName)}}
              UNION
              {{CreateScript(SuspiciousCasesType.CurrentAddressLinkedWithBuildingUnitButNotWithParcel, CurrentAddressLinkedWithBuildingUnitButNotWithParcelConfiguration.ViewName)}}
              UNION
              {{CreateScript(SuspiciousCasesType.CurrentAddressWithoutLinkedParcelsOrBuildingUnits, CurrentAddressWithoutLinkedParcelOrBuildingUnitConfiguration.ViewName)}}
              UNION
              {{CreateScript(SuspiciousCasesType.CurrentAddressesWithSpecificationDerivedFromBuildingUnitWithoutLinkedBuildingUnit, CurrentAddressWithSpecificationDerivedFromBuildingUnitWithoutLinkedBuildingUnitConfiguration.ViewName)}}
              UNION
              {{CreateScript(SuspiciousCasesType.CurrentStreetNameWithoutLinkedRoadSegment, CurrentStreetNameWithoutLinkedRoadSegmentsConfiguration.ViewName)}}
              UNION
              {{CreateScript(SuspiciousCasesType.ProposedAddressWithoutLinkedParcelOrBuildingUnit, ProposedAddressWithoutLinkedParcelOrBuildingUnitConfiguration.ViewName)}}
              UNION
              {{CreateScript(SuspiciousCasesType.RoadSegmentLongerThanTwoYearsWithPermit, RoadSegmentLongerThanTwoYearsWithPermitConfiguration.ViewName)}}
              UNION
              {{CreateScript(SuspiciousCasesType.StreetNameLongerThanTwoYearsProposed, StreetNameLongerThanTwoYearsProposedConfiguration.ViewName)}}
              ;

              CREATE INDEX ix_{{ViewName}}_nis_code ON {{Schema.SuspiciousCases}}.{{ViewName}} USING btree (nis_code);
              """;

        private static string CreateScript(SuspiciousCasesType type, string viewName)
        {
            return $@"SELECT
                    nis_code,
                    count(*) as count,
                    {(int)type} as type
                    FROM {Schema.SuspiciousCases}.{viewName}
                    GROUP BY nis_code
                ";
        }
    }
}
