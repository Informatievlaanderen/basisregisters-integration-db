using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Basisregisters.IntegrationDb.SuspiciousCases.Migrations
{
    using Views;
    using Views.Internal;

    /// <inheritdoc />
    public partial class SwitchAddressTableV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(ActiveAddressLinkedToMultipleBuildingUnitsConfiguration.Create);
            migrationBuilder.Sql(ActiveAddressOutsideMunicipalityBoundsConfiguration.Create);
            migrationBuilder.Sql(AddressLongerThanTwoYearsProposedConfiguration.Create);
            migrationBuilder.Sql(CurrentAddressLinkedToProposedStreetNameConfiguration.Create);
            migrationBuilder.Sql(CurrentAddressLinkedWithBuildingUnitButNotWithParcelConfiguration.Create);
            migrationBuilder.Sql(CurrentAddressWithSpecificationDerivedFromBuildingUnitWithoutLinkedBuildingUnitConfiguration.Create);
            migrationBuilder.Sql(CurrentAddressWithoutLinkedParcelOrBuildingUnitConfiguration.Create);
            migrationBuilder.Sql(ProposedAddressWithoutLinkedParcelOrBuildingUnitConfiguration.Create);
            migrationBuilder.Sql(ActiveAddressButInActiveStreetName.Create);
            migrationBuilder.Sql(InactiveAddressLinkedToParcelOrBuildingUnit.Create);
            migrationBuilder.Sql(InactiveParcelLinkedToAddress.Create);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
