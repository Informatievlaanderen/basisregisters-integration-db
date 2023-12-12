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
            migrationBuilder.Sql(Views.BuildingUnitAddressRelations.Create);
            migrationBuilder.Sql(Views.ParcelAddressRelations.Create);

            migrationBuilder.Sql(Views.AddressesLinkedToMultipleBuildingUnits.Create);
            migrationBuilder.Sql(Views.AddressesLinkedToMultipleParcels.Create);
            migrationBuilder.Sql(Views.AddressesWithMultipleLinks.Create);
            migrationBuilder.Sql(Views.AddressesWithoutPostalCode.Create);

            migrationBuilder.Sql(Views.ParcelsLinkedToMultipleAddresses.Create);

            migrationBuilder.Sql(Views.CurrentAddressWithoutLinkedParcels.Create);
            migrationBuilder.Sql(Views.CurrentAddressWithoutLinkedParcelOrBuildingUnit.Create);
            migrationBuilder.Sql(Views.CurrentStreetnameWithoutLinkedRoadSegments.Create);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string DropView(string viewName) => @$"DROP VIEW {viewName}";

            migrationBuilder.Sql(DropView(nameof(Views.AddressesWithMultipleLinks.Table)));
            migrationBuilder.Sql(DropView(nameof(Views.AddressesLinkedToMultipleBuildingUnits.Table)));
            migrationBuilder.Sql(DropView(nameof(Views.AddressesLinkedToMultipleParcels.Table)));
            migrationBuilder.Sql(DropView(nameof(Views.AddressesWithoutPostalCode.Table)));

            migrationBuilder.Sql(DropView(nameof(Views.CurrentAddressWithoutLinkedParcels.Table)));
            migrationBuilder.Sql(DropView(nameof(Views.CurrentAddressWithoutLinkedParcelOrBuildingUnit.Table)));

            migrationBuilder.Sql(DropView(nameof(Views.ParcelsLinkedToMultipleAddresses.Table)));

            migrationBuilder.Sql(DropView(nameof(Views.CurrentStreetnameWithoutLinkedRoadSegments.Table)));

            migrationBuilder.Sql(DropView(nameof(Views.BuildingUnitAddressRelations.Table)));
            migrationBuilder.Sql(DropView(nameof(Views.ParcelAddressRelations.Table)));
        }
    }
}
