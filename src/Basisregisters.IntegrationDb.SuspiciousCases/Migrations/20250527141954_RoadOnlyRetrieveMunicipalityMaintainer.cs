using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Basisregisters.IntegrationDb.SuspiciousCases.Migrations
{
    using Views;

    /// <inheritdoc />
    public partial class RoadOnlyRetrieveMunicipalityMaintainer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(SuspiciousCaseCountConfiguration.Drop);

            migrationBuilder.Sql(RoadSegmentLinkedToRetiredStreetNameConfiguration.Create);
            migrationBuilder.Sql(RoadSegmentLongerThanTwoYearsWithPermitConfiguration.Create);
            migrationBuilder.Sql(RoadSegmentWithSingleLinkedStreetNameConfiguration.Create);

            migrationBuilder.Sql(SuspiciousCaseCountConfiguration.Create);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
