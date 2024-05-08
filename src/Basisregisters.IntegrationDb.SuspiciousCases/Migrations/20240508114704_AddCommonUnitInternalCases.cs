using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Basisregisters.IntegrationDb.SuspiciousCases.Migrations
{
    using Views.Internal;

    /// <inheritdoc />
    public partial class AddCommonUnitInternalCases : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"DROP VIEW {InactiveBuildingUnitLinkedToAddress.ViewName};");
            migrationBuilder.Sql(InactiveBuildingUnitLinkedToAddress.Create);

            migrationBuilder.Sql(BuildingWithMoreThanOneUnitWithoutCommonUnit.Create);
            migrationBuilder.Sql(BuildingWithOneOrNoUnitWithCommonUnit.Create);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"DROP VIEW {InactiveBuildingUnitLinkedToAddress.ViewName};");
            migrationBuilder.Sql($"DROP VIEW {BuildingWithMoreThanOneUnitWithoutCommonUnit.ViewName};");
            migrationBuilder.Sql($"DROP VIEW {BuildingWithOneOrNoUnitWithCommonUnit.ViewName};");
        }
    }
}
