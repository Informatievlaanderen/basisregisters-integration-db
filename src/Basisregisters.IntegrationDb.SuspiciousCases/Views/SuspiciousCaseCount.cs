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

        public static readonly string Create =
            @$"CREATE MATERIALIZED VIEW IF NOT EXISTS {Schema.SuspiciousCases}.{ViewName} AS
                {CreateScript(SuspiciousCasesType.StreetNamesLongerThanTwoYearsProposed, StreetNamesLongerThanTwoYearsProposedConfiguration.ViewName)}
                UNION
                {CreateScript(SuspiciousCasesType.ActiveBuildingUnitsLinkedToMultipleAddresses, ActiveBuildingUnitLinkedToMultipleAddressesConfiguration.ViewName)};
                CREATE INDEX ""ix_{ViewName}_nis_code"" ON {Schema.SuspiciousCases}.{ViewName} USING btree (nis_code)
            ;";

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
