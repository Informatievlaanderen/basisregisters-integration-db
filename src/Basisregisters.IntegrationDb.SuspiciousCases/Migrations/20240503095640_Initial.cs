using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Basisregisters.IntegrationDb.SuspiciousCases.Migrations
{
    using Functions;
    using Views;
    using Infrastructure;
    using Views.Internal;

    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.Sql(GetFullAddress.Create);

            migrationBuilder.Sql(ActiveAddressLinkedToMultipleBuildingUnitsConfiguration.Create);
            migrationBuilder.Sql(ActiveAddressOutsideMunicipalityBoundsConfiguration.Create);
            migrationBuilder.Sql(ActiveBuildingUnitLinkedToMultipleAddressesConfiguration.Create);
            migrationBuilder.Sql(ActiveBuildingUnitWithoutAddressConfiguration.Create);
            migrationBuilder.Sql(AddressLongerThanTwoYearsProposedConfiguration.Create);
            migrationBuilder.Sql(BuildingsLongerThanTwoYearsPlannedConfiguration.Create);
            migrationBuilder.Sql(BuildingUnitsLongerThanTwoYearsPlannedConfiguration.Create);
            migrationBuilder.Sql(CurrentAddressLinkedWithBuildingUnitButNotWithParcelConfiguration.Create);
            migrationBuilder.Sql(CurrentAddressWithoutLinkedParcelOrBuildingUnitConfiguration.Create);
            migrationBuilder.Sql(CurrentAddressWithSpecificationDerivedFromBuildingUnitWithoutLinkedBuildingUnitConfiguration.Create);
            migrationBuilder.Sql(ProposedAddressWithoutLinkedParcelOrBuildingUnitConfiguration.Create);
            migrationBuilder.Sql(StreetNameLongerThanTwoYearsProposedConfiguration.Create);

            migrationBuilder.Sql(SuspiciousCaseCountConfiguration.Create);

            migrationBuilder.Sql(ActiveBuildingUnitOutsideBuilding.Create);
            migrationBuilder.Sql(InactiveAddressLinkedToParcelOrBuildingUnit.Create);
            migrationBuilder.Sql(InactiveBuildingUnitLinkedToAddress.Create);
            migrationBuilder.Sql(InactiveParcelLinkedToAddress.Create);
            migrationBuilder.Sql(ActiveAddressButInActiveStreetName.Create);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"DROP VIEW {ActiveBuildingUnitOutsideBuilding.ViewName};");
            migrationBuilder.Sql($"DROP VIEW {InactiveAddressLinkedToParcelOrBuildingUnit.ViewName};");
            migrationBuilder.Sql($"DROP VIEW {InactiveBuildingUnitLinkedToAddress.ViewName};");
            migrationBuilder.Sql($"DROP VIEW {InactiveParcelLinkedToAddress.ViewName};");
            migrationBuilder.Sql($"DROP VIEW {ActiveAddressButInActiveStreetName.ViewName};");

            migrationBuilder.Sql($"DROP VIEW {SuspiciousCaseCountConfiguration.ViewName};");

            migrationBuilder.Sql($"DROP VIEW {ActiveAddressLinkedToMultipleBuildingUnitsConfiguration.ViewName};");
            migrationBuilder.Sql($"DROP VIEW {ActiveAddressOutsideMunicipalityBoundsConfiguration.ViewName};");
            migrationBuilder.Sql($"DROP VIEW {ActiveBuildingUnitLinkedToMultipleAddressesConfiguration.ViewName};");
            migrationBuilder.Sql($"DROP VIEW {ActiveBuildingUnitWithoutAddressConfiguration.ViewName};");
            migrationBuilder.Sql($"DROP VIEW {AddressLongerThanTwoYearsProposedConfiguration.ViewName};");
            migrationBuilder.Sql($"DROP VIEW {BuildingsLongerThanTwoYearsPlannedConfiguration.ViewName};");
            migrationBuilder.Sql($"DROP VIEW {BuildingUnitsLongerThanTwoYearsPlannedConfiguration.ViewName};");
            migrationBuilder.Sql($"DROP VIEW {CurrentAddressLinkedWithBuildingUnitButNotWithParcelConfiguration.ViewName};");
            migrationBuilder.Sql($"DROP VIEW {CurrentAddressWithoutLinkedParcelOrBuildingUnitConfiguration.ViewName};");
            migrationBuilder.Sql($"DROP VIEW {CurrentAddressWithSpecificationDerivedFromBuildingUnitWithoutLinkedBuildingUnitConfiguration.ViewName};");
            migrationBuilder.Sql($"DROP VIEW {ProposedAddressWithoutLinkedParcelOrBuildingUnitConfiguration.ViewName};");
            migrationBuilder.Sql($"DROP VIEW {StreetNameLongerThanTwoYearsProposedConfiguration.ViewName};");

            migrationBuilder.Sql($"DROP FUNCTION {Schema.FullAddress};");
        }
    }
}
