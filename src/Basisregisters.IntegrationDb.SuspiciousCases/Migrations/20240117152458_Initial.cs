using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Basisregisters.IntegrationDb.SuspiciousCases.Migrations
{
    using Functions;
    using Schema.Views.SuspiciousCases;
    using Views;

    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(GetFullAddress.Create);

            migrationBuilder.Sql(CurrentAddressWithoutLinkedParcelOrBuildingUnitsConfiguration.Create);
            migrationBuilder.Sql(ProposedAddressWithoutLinkedParcelOrBuildingUnitsConfiguration.Create);
            migrationBuilder.Sql(StreetNamesLongerThanTwoYearsProposedConfiguration.Create);
            migrationBuilder.Sql(AddressesLongerThanTwoYearsProposedConfiguration.Create);
            migrationBuilder.Sql(BuildingsLongerThanTwoYearsPlannedConfiguration.Create);
            migrationBuilder.Sql(BuildingUnitsLongerThanTwoYearsPlannedConfiguration.Create);
            migrationBuilder.Sql(ActiveBuildingUnitWithoutAddressConfiguration.Create);
            migrationBuilder.Sql(AddressesLinkedToMultipleBuildingUnitsConfiguration.Create);
            migrationBuilder.Sql(CurrentAddressesWithSpecificationDerivedFromObjectWithoutBuildingUnitConfiguration.Create);
            migrationBuilder.Sql(CurrentAddressesOutsideMunicipalityBoundsConfiguration.Create);
            migrationBuilder.Sql(ActiveBuildingUnitLinkedToMultipleAddressesConfiguration.Create);
            migrationBuilder.Sql(SuspiciousCaseCountConfiguration.Create);

            // TODO: enable when roadsegment is avaialable
            //migrationBuilder.Sql(CurrentStreetNameWithoutLinkedRoadSegmentsConfiguration.Create);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
