using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Basisregisters.IntegrationDb.SuspiciousCases.Migrations
{
    using Infrastructure;
    using Views;
    using Views.Internal;

    /// <inheritdoc />
    public partial class RecreateParcelViews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"DROP MATERIALIZED VIEW IF EXISTS {Schema.SuspiciousCases}.{SuspiciousCaseCountConfiguration.ViewName};");

            migrationBuilder.Sql($"DROP VIEW IF EXISTS {Schema.SuspiciousCases}.{InactiveParcelLinkedToAddress.ViewName};");
            migrationBuilder.Sql(InactiveParcelLinkedToAddress.Create);

            migrationBuilder.Sql($"DROP VIEW IF EXISTS {Schema.SuspiciousCases}.{InactiveAddressLinkedToParcelOrBuildingUnit.ViewName};");
            migrationBuilder.Sql(InactiveAddressLinkedToParcelOrBuildingUnit.Create);

            migrationBuilder.Sql($"DROP VIEW IF EXISTS {Schema.SuspiciousCases}.{CurrentAddressLinkedWithBuildingUnitButNotWithParcelConfiguration.ViewName};");
            migrationBuilder.Sql(CurrentAddressLinkedWithBuildingUnitButNotWithParcelConfiguration.Create);

            migrationBuilder.Sql($"DROP VIEW IF EXISTS {Schema.SuspiciousCases}.{CurrentAddressWithoutLinkedParcelOrBuildingUnitConfiguration.ViewName};");
            migrationBuilder.Sql(CurrentAddressWithoutLinkedParcelOrBuildingUnitConfiguration.Create);

            migrationBuilder.Sql($"DROP VIEW IF EXISTS {Schema.SuspiciousCases}.{ProposedAddressWithoutLinkedParcelOrBuildingUnitConfiguration.ViewName};");
            migrationBuilder.Sql(ProposedAddressWithoutLinkedParcelOrBuildingUnitConfiguration.Create);

            migrationBuilder.Sql(SuspiciousCaseCountConfiguration.Create);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(SuspiciousCaseCountConfiguration.Create);
        }
    }
}
