using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Basisregisters.IntegrationDb.SuspiciousCases.Migrations
{
    using Infrastructure;
    using Views;

    /// <inheritdoc />
    public partial class AddRoadSegmentViews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(CurrentStreetNameWithoutLinkedRoadSegmentsConfiguration.Create);
            migrationBuilder.Sql(RoadSegmentLongerThanTwoYearsWithPermitConfiguration.Create);
            migrationBuilder.Sql(ActiveAddressOutsideMunicipalityBoundsConfiguration.Create);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"DROP VIEW {Schema.SuspiciousCases}.{CurrentStreetNameWithoutLinkedRoadSegmentsConfiguration.ViewName};");
            migrationBuilder.Sql($"DROP VIEW {Schema.SuspiciousCases}.{RoadSegmentLongerThanTwoYearsWithPermitConfiguration.ViewName};");
        }
    }
}
