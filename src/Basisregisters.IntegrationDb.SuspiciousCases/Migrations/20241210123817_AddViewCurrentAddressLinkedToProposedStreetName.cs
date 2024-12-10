using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Basisregisters.IntegrationDb.SuspiciousCases.Migrations
{
    using System;
    using Infrastructure;
    using Views;

    /// <inheritdoc />
    public partial class AddViewCurrentAddressLinkedToProposedStreetName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"DROP VIEW IF EXISTS {Schema.SuspiciousCases}.{CurrentAddressLinkedToProposedStreetNameConfiguration.ViewName};");

            migrationBuilder.Sql(CurrentAddressLinkedToProposedStreetNameConfiguration.Create);

            migrationBuilder.Sql(SuspiciousCaseCountConfiguration.Create);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            throw new NotSupportedException();
        }
    }
}
