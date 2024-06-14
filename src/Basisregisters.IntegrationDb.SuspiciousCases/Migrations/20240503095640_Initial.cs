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
            migrationBuilder.Sql(BuildingLongerThanTwoYearsPlannedConfiguration.Create);
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
            migrationBuilder.Sql($"DROP VIEW {Schema.SuspiciousCases}.{ActiveBuildingUnitOutsideBuilding.ViewName};");
            migrationBuilder.Sql($"DROP VIEW {Schema.SuspiciousCases}.{InactiveAddressLinkedToParcelOrBuildingUnit.ViewName};");
            migrationBuilder.Sql($"DROP VIEW {Schema.SuspiciousCases}.{InactiveBuildingUnitLinkedToAddress.ViewName};");
            migrationBuilder.Sql($"DROP VIEW {Schema.SuspiciousCases}.{InactiveParcelLinkedToAddress.ViewName};");
            migrationBuilder.Sql($"DROP VIEW {Schema.SuspiciousCases}.{ActiveAddressButInActiveStreetName.ViewName};");

            migrationBuilder.Sql($"DROP VIEW {Schema.SuspiciousCases}.{SuspiciousCaseCountConfiguration.ViewName};");

            migrationBuilder.Sql($"DROP VIEW {Schema.SuspiciousCases}.{ActiveAddressLinkedToMultipleBuildingUnitsConfiguration.ViewName};");
            migrationBuilder.Sql($"DROP VIEW {Schema.SuspiciousCases}.{ActiveAddressOutsideMunicipalityBoundsConfiguration.ViewName};");
            migrationBuilder.Sql($"DROP VIEW {Schema.SuspiciousCases}.{ActiveBuildingUnitLinkedToMultipleAddressesConfiguration.ViewName};");
            migrationBuilder.Sql($"DROP VIEW {Schema.SuspiciousCases}.{ActiveBuildingUnitWithoutAddressConfiguration.ViewName};");
            migrationBuilder.Sql($"DROP VIEW {Schema.SuspiciousCases}.{AddressLongerThanTwoYearsProposedConfiguration.ViewName};");
            migrationBuilder.Sql($"DROP VIEW {Schema.SuspiciousCases}.{BuildingLongerThanTwoYearsPlannedConfiguration.ViewName};");
            migrationBuilder.Sql($"DROP VIEW {Schema.SuspiciousCases}.{BuildingUnitsLongerThanTwoYearsPlannedConfiguration.ViewName};");
            migrationBuilder.Sql($"DROP VIEW {Schema.SuspiciousCases}.{CurrentAddressLinkedWithBuildingUnitButNotWithParcelConfiguration.ViewName};");
            migrationBuilder.Sql($"DROP VIEW {Schema.SuspiciousCases}.{CurrentAddressWithoutLinkedParcelOrBuildingUnitConfiguration.ViewName};");
            migrationBuilder.Sql($"DROP VIEW {Schema.SuspiciousCases}.{CurrentAddressWithSpecificationDerivedFromBuildingUnitWithoutLinkedBuildingUnitConfiguration.ViewName};");
            migrationBuilder.Sql($"DROP VIEW {Schema.SuspiciousCases}.{ProposedAddressWithoutLinkedParcelOrBuildingUnitConfiguration.ViewName};");
            migrationBuilder.Sql($"DROP VIEW {Schema.SuspiciousCases}.{StreetNameLongerThanTwoYearsProposedConfiguration.ViewName};");

            migrationBuilder.Sql($"DROP FUNCTION {Schema.FullAddress};");
        }
    }
}
