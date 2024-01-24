using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Basisregisters.IntegrationDb.SuspiciousCases.Migrations
{
    using Views;

    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(StreetNamesLongerThanTwoYearsProposedConfiguration.Create);
            migrationBuilder.Sql(SuspiciousCaseCountConfiguration.Create);
            migrationBuilder.Sql(CurrentAddressesOutsideMunicipalityBoundsConfiguration.Create);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
