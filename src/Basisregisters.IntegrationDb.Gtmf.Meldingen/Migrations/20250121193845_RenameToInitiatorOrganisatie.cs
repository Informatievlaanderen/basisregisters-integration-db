using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Basisregisters.IntegrationDb.Gtmf.Meldingen.Migrations
{
    /// <inheritdoc />
    public partial class RenameToInitiatorOrganisatie : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "organisatie_id_internal",
                schema: "integration_gtmf",
                table: "meldingsobject_statuswijziging",
                newName: "initiator_organisatie_id_internal");

            migrationBuilder.RenameIndex(
                name: "IX_meldingsobject_statuswijziging_organisatie_id_internal",
                schema: "integration_gtmf",
                table: "meldingsobject_statuswijziging",
                newName: "IX_meldingsobject_statuswijziging_initiator_organisatie_id_int~");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "initiator_organisatie_id_internal",
                schema: "integration_gtmf",
                table: "meldingsobject_statuswijziging",
                newName: "organisatie_id_internal");

            migrationBuilder.RenameIndex(
                name: "IX_meldingsobject_statuswijziging_initiator_organisatie_id_int~",
                schema: "integration_gtmf",
                table: "meldingsobject_statuswijziging",
                newName: "IX_meldingsobject_statuswijziging_organisatie_id_internal");
        }
    }
}
