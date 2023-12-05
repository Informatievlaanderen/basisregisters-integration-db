using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Basisregisters.IntegrationDb.Schema.Migrations
{
    using Models;
    using Models.Views;

    public partial class add_views : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(ViewQueries.VIEW_BuildingUnitAddressRelations);
            migrationBuilder.Sql(ViewQueries.VIEW_ParcelAddressRelations);

            migrationBuilder.Sql(ViewQueries.VIEW_AddressesLinkedToMultipleBuildingUnits);
            migrationBuilder.Sql(ViewQueries.VIEW_ParcelsLinkedToMultipleAddresses);
            migrationBuilder.Sql(ViewQueries.VIEW_ActiveAddressWithoutLinkedParcels);
            migrationBuilder.Sql(ViewQueries.VIEW_ActiveAddressWithoutLinkedParcelOrBuildingUnit);
            migrationBuilder.Sql(ViewQueries.VIEW_AddressesLinkedToMultipleParcels);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string DropView(string viewName) => @$"DROP VIEW ""{IntegrationContext.Schema}"".""{viewName}""";

            migrationBuilder.Sql(DropView(nameof(ViewQueries.VIEW_AddressesLinkedToMultipleBuildingUnits)));
            migrationBuilder.Sql(DropView(nameof(ViewQueries.VIEW_ParcelsLinkedToMultipleAddresses)));
            migrationBuilder.Sql(DropView(nameof(ViewQueries.VIEW_ActiveAddressWithoutLinkedParcels)));
            migrationBuilder.Sql(DropView(nameof(ViewQueries.VIEW_ActiveAddressWithoutLinkedParcelOrBuildingUnit)));
            migrationBuilder.Sql(DropView(nameof(ViewQueries.VIEW_AddressesLinkedToMultipleParcels)));

            migrationBuilder.Sql(DropView(nameof(ViewQueries.VIEW_BuildingUnitAddressRelations)));
            migrationBuilder.Sql(DropView(nameof(ViewQueries.VIEW_ParcelAddressRelations)));
        }
    }
}
