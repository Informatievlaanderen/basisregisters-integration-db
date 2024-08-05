using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Basisregisters.IntegrationDb.SuspiciousCases.Migrations
{
    using Infrastructure;
    using Views;

    /// <inheritdoc />
    public partial class DropAddRoadSegmentLinkedStreetNameViews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"DROP VIEW IF EXISTS {Schema.SuspiciousCases}.view_measured_road_segment_with_no_or_single_linked_streetname;");

            migrationBuilder.Sql(RoadSegmentWithSingleLinkedStreetNameConfiguration.Create);

            migrationBuilder.Sql(SuspiciousCaseCountConfiguration.Create);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"DROP VIEW {Schema.SuspiciousCases}.{SuspiciousCaseCountConfiguration.ViewName};");

            migrationBuilder.Sql($"DROP VIEW {Schema.SuspiciousCases}.{RoadSegmentWithSingleLinkedStreetNameConfiguration.ViewName};");
        }
    }
}
