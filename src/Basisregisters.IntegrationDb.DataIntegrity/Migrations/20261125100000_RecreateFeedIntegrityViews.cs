using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Basisregisters.IntegrationDb.DataIntegrity.Migrations
{
    using Feeds;

    /// <inheritdoc />
    public partial class RecreateFeedIntegrityViews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(MunicipalityViewRepository.DropAndCreateMunicipalityViewSql);
            migrationBuilder.Sql(PostalViewRepository.DropAndCreatePostalViewSql);
            migrationBuilder.Sql(PostalNameViewRepository.DropAndCreatePostalNameViewSql);
            migrationBuilder.Sql(StreetNameViewRepository.DropAndCreateStreetNameViewSql);
            migrationBuilder.Sql(AddressViewRepository.DropAndCreateAddressViewSql);
            migrationBuilder.Sql(BuildingViewRepository.DropAndCreateBuildingViewSql);
            migrationBuilder.Sql(BuildingUnitViewRepository.DropAndCreateBuildingUnitViewSql);
            migrationBuilder.Sql(BuildingUnitAddressViewRepository.DropAndCreateBuildingUnitAddressViewSql);
            migrationBuilder.Sql(ParcelViewRepository.DropAndCreateParcelViewSql);
            migrationBuilder.Sql(ParcelAddressViewRepository.DropAndCreateParcelAddressViewSql);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // The previous view definitions are not kept; the views are recreated with the current definitions.
        }
    }
}
