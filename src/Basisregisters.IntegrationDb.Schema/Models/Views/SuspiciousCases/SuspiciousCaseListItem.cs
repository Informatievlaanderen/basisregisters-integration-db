namespace Basisregisters.IntegrationDb.Schema.Models.Views.SuspiciousCases
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Microsoft.EntityFrameworkCore.Metadata.Internal;

    public class SuspiciousCaseListItem
    {
        public int NisCode { get; set; }
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
                .ToView(nameof(SuspiciousCaseListItem), IntegrationContext.Schema)
                .HasNoKey()
                .ToSqlQuery(@$"select
                                ""NisCode""
                                ,""Count""
                                ,""Type""
                            FROM {ViewName}");
        }

        public const string ViewName = @$"""{IntegrationContext.Schema}"".""VIEW_{nameof(SuspiciousCaseListItemConfiguration)}""";

        public static readonly string Create =
            @$"CREATE MATERIALIZED VIEW IF NOT EXISTS {ViewName} AS
                {CreateScript(SuspiciousCasesType.AddressesLinkedToMultipleBuildingUnits, AddressesLinkedToMultipleBuildingUnitsConfiguration.ViewName)}
                Union
                {CreateScript(SuspiciousCasesType.AddressesOutsideOfMunicipalityBoundaries, CurrentAddressesOutsideMunicipalityBoundsConfiguration.ViewName)}
                Union
                {CreateScript(SuspiciousCasesType.CurrentAddressWithoutLinkedParcelsOrBuildingUnits, CurrentAddressWithoutLinkedParcelOrBuildingUnitsConfiguration.ViewName)}
                Union
                {CreateScript(SuspiciousCasesType.CurrentStreetNamesWithoutRoadSegment, CurrentStreetNameWithoutLinkedRoadSegmentsConfiguration.ViewName)}
                Union
                {CreateScript(SuspiciousCasesType.ProposedAddressWithoutLinkedParcelsOrBuildingUnits, ProposedAddressWithoutLinkedParcelOrBuildingUnitsConfiguration.ViewName)}
                ;
                CREATE INDEX ""IX_{nameof(SuspiciousCaseListItem)}_NisCode"" ON {ViewName} USING btree (""{nameof(SuspiciousCaseListItem.NisCode)}"");
            ";

        private static string CreateScript(SuspiciousCasesType type, string viewName)
        {
            return $@"SELECT
                    ""NisCode"",
                    count(*) as ""Count"",
                    {(int)type} as ""Type""
                    FROM {viewName}
                    GROUP BY ""NisCode""
                ";
        }
    }
}
