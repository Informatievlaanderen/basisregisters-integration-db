using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Basisregisters.IntegrationDb.SuspiciousCases.Migrations
{
    using Functions;
    using Infrastructure;
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"DROP VIEW {SuspiciousCaseCountConfiguration.ViewName};");
            migrationBuilder.Sql($"DROP VIEW {ActiveBuildingUnitLinkedToMultipleAddressesConfiguration.ViewName};");
            migrationBuilder.Sql($"DROP VIEW {CurrentAddressesOutsideMunicipalityBoundsConfiguration.ViewName};");
            migrationBuilder.Sql($"DROP VIEW {CurrentAddressesWithSpecificationDerivedFromObjectWithoutBuildingUnitConfiguration.ViewName};");
            migrationBuilder.Sql($"DROP VIEW {AddressesLinkedToMultipleBuildingUnitsConfiguration.ViewName};");
            migrationBuilder.Sql($"DROP VIEW {ActiveBuildingUnitWithoutAddressConfiguration.ViewName};");
            migrationBuilder.Sql($"DROP VIEW {BuildingUnitsLongerThanTwoYearsPlannedConfiguration.ViewName};");
            migrationBuilder.Sql($"DROP VIEW {BuildingsLongerThanTwoYearsPlannedConfiguration.ViewName};");
            migrationBuilder.Sql($"DROP VIEW {AddressesLongerThanTwoYearsProposedConfiguration.ViewName};");
            migrationBuilder.Sql($"DROP VIEW {StreetNamesLongerThanTwoYearsProposedConfiguration.ViewName};");
            migrationBuilder.Sql($"DROP VIEW {ProposedAddressWithoutLinkedParcelOrBuildingUnitsConfiguration.ViewName};");
            migrationBuilder.Sql($"DROP VIEW {CurrentAddressWithoutLinkedParcelOrBuildingUnitsConfiguration.ViewName};");

            migrationBuilder.Sql($"DROP FUNCTION {Schema.FullAddress};");
        }
    }
}
