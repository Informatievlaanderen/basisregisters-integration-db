using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Basisregisters.IntegrationDb.Schema.Migrations
{
    using SuspiciousCases.Views;

    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(StreetNamesLongerThanTwoYearsProposedConfiguration.Create);
            migrationBuilder.Sql(SuspiciousCaseCountConfiguration.Create);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
