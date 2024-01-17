using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Basisregisters.IntegrationDb.Schema.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(Views.SuspiciousCases.StreetNamesLongerThanTwoYearsProposedConfiguration.Create);
            migrationBuilder.Sql(Views.SuspiciousCases.SuspiciousCaseListItemConfiguration.Create);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
