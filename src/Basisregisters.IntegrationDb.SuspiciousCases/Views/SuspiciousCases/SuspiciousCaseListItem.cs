﻿namespace Basisregisters.IntegrationDb.Schema.Views.SuspiciousCases
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class SuspiciousCaseListItem
    {
        public string NisCode { get; set; }
        public int Count { get; set; }
        public SuspiciousCasesType Type { get; set; }

        public SuspiciousCaseListItem()
        { }
    }

    public sealed class SuspiciousCaseListItemConfiguration : IEntityTypeConfiguration<SuspiciousCaseListItem>
    {
        public void Configure(EntityTypeBuilder<SuspiciousCaseListItem> builder)
        {
            builder
                .ToView(ViewName, Schema)
                .HasNoKey()
                .ToSqlQuery(@$"select
                                nis_code,
                                count,
                                type
                            FROM {Schema}.{ViewName}");

            builder.Property(x => x.NisCode)
                .HasColumnName("nis_code");
            builder.Property(x => x.Count)
                .HasColumnName("count");
            builder.Property(x => x.Type)
                .HasColumnName("type");
        }

        public const string Schema = SuspiciousCasesContext.SchemaSuspiciousCases;
        public const string ViewName = "view_suspicious_cases_list_item";

        public static readonly string Create =
            @$"CREATE MATERIALIZED VIEW IF NOT EXISTS {Schema}.{ViewName} AS
                {CreateScript(SuspiciousCasesType.StreetNamesLongerThanTwoYearsProposed, StreetNamesLongerThanTwoYearsProposedConfiguration.ViewName)}
                ;
                CREATE INDEX ""ix_{ViewName}_nis_code"" ON {Schema}.{ViewName} USING btree (nis_code);
            ";

        private static string CreateScript(SuspiciousCasesType type, string viewName)
        {
            return $@"SELECT
                    nis_code,
                    count(*) as count,
                    {(int)type} as type
                    FROM {Schema}.{viewName}
                    GROUP BY nis_code
                ";
        }
    }
}
