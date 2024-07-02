using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace Basisregisters.IntegrationDb.Gtmf.Meldingen.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "integration_gtmf");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.CreateTable(
                name: "meldingsobject",
                schema: "integration_gtmf",
                columns: table => new
                {
                    meldingsobject_id = table.Column<Guid>(type: "uuid", nullable: false),
                    melding_id = table.Column<Guid>(type: "uuid", nullable: false),
                    datum_indiening_as_string = table.Column<string>(type: "text", nullable: false),
                    datum_indiening = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    datum_vaststelling_as_string = table.Column<string>(type: "text", nullable: false),
                    datum_vaststelling = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    meldingsorganisatie_id_internal = table.Column<Guid>(type: "uuid", nullable: false),
                    meldingsorganisatie_id = table.Column<Guid>(type: "uuid", nullable: false),
                    meldingsapplicatie = table.Column<string>(type: "text", nullable: false),
                    referentie = table.Column<string>(type: "text", nullable: false),
                    referentie_melder = table.Column<string>(type: "text", nullable: true),
                    onderwerp = table.Column<string>(type: "text", nullable: true),
                    beschrijving = table.Column<string>(type: "text", nullable: true),
                    samenvatting = table.Column<string>(type: "text", nullable: true),
                    thema = table.Column<string>(type: "text", nullable: false),
                    oorzaak = table.Column<string>(type: "text", nullable: false),
                    ovo_code = table.Column<string>(type: "text", nullable: false),
                    geometrie = table.Column<Geometry>(type: "geometry", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_meldingsobject", x => x.meldingsobject_id);
                });

            migrationBuilder.CreateTable(
                name: "meldingsobject_statuswijziging",
                schema: "integration_gtmf",
                columns: table => new
                {
                    meldingsobject_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nieuwe_status = table.Column<string>(type: "text", nullable: false),
                    melding_id = table.Column<Guid>(type: "uuid", nullable: false),
                    oude_status = table.Column<string>(type: "text", nullable: true),
                    organisatie_id_internal = table.Column<Guid>(type: "uuid", nullable: false),
                    tijdstip_wijziging_as_string = table.Column<string>(type: "text", nullable: false),
                    tijdstip_wijziging = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    toelichting = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_meldingsobject_statuswijziging", x => new { x.meldingsobject_id, x.nieuwe_status });
                });

            migrationBuilder.CreateTable(
                name: "organisatie",
                schema: "integration_gtmf",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    id_internal = table.Column<Guid>(type: "uuid", nullable: false),
                    naam = table.Column<string>(type: "text", nullable: false),
                    ovo_code = table.Column<string>(type: "text", nullable: true),
                    kbo_nummer = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_organisatie", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "projection_state",
                schema: "integration_gtmf",
                columns: table => new
                {
                    naam = table.Column<string>(type: "text", nullable: false),
                    position = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_projection_state", x => x.naam);
                });

            migrationBuilder.CreateIndex(
                name: "IX_meldingsobject_melding_id",
                schema: "integration_gtmf",
                table: "meldingsobject",
                column: "melding_id");

            migrationBuilder.CreateIndex(
                name: "IX_meldingsobject_meldingsapplicatie",
                schema: "integration_gtmf",
                table: "meldingsobject",
                column: "meldingsapplicatie");

            migrationBuilder.CreateIndex(
                name: "IX_meldingsobject_meldingsorganisatie_id",
                schema: "integration_gtmf",
                table: "meldingsobject",
                column: "meldingsorganisatie_id");

            migrationBuilder.CreateIndex(
                name: "IX_meldingsobject_meldingsorganisatie_id_internal",
                schema: "integration_gtmf",
                table: "meldingsobject",
                column: "meldingsorganisatie_id_internal");

            migrationBuilder.CreateIndex(
                name: "IX_meldingsobject_onderwerp",
                schema: "integration_gtmf",
                table: "meldingsobject",
                column: "onderwerp");

            migrationBuilder.CreateIndex(
                name: "IX_meldingsobject_oorzaak",
                schema: "integration_gtmf",
                table: "meldingsobject",
                column: "oorzaak");

            migrationBuilder.CreateIndex(
                name: "IX_meldingsobject_ovo_code",
                schema: "integration_gtmf",
                table: "meldingsobject",
                column: "ovo_code");

            migrationBuilder.CreateIndex(
                name: "IX_meldingsobject_referentie",
                schema: "integration_gtmf",
                table: "meldingsobject",
                column: "referentie");

            migrationBuilder.CreateIndex(
                name: "IX_meldingsobject_referentie_melder",
                schema: "integration_gtmf",
                table: "meldingsobject",
                column: "referentie_melder");

            migrationBuilder.CreateIndex(
                name: "IX_meldingsobject_thema",
                schema: "integration_gtmf",
                table: "meldingsobject",
                column: "thema");

            migrationBuilder.CreateIndex(
                name: "IX_meldingsobject_statuswijziging_melding_id",
                schema: "integration_gtmf",
                table: "meldingsobject_statuswijziging",
                column: "melding_id");

            migrationBuilder.CreateIndex(
                name: "IX_meldingsobject_statuswijziging_nieuwe_status",
                schema: "integration_gtmf",
                table: "meldingsobject_statuswijziging",
                column: "nieuwe_status");

            migrationBuilder.CreateIndex(
                name: "IX_meldingsobject_statuswijziging_organisatie_id_internal",
                schema: "integration_gtmf",
                table: "meldingsobject_statuswijziging",
                column: "organisatie_id_internal");

            migrationBuilder.CreateIndex(
                name: "IX_meldingsobject_statuswijziging_oude_status",
                schema: "integration_gtmf",
                table: "meldingsobject_statuswijziging",
                column: "oude_status");

            migrationBuilder.CreateIndex(
                name: "IX_organisatie_id_internal",
                schema: "integration_gtmf",
                table: "organisatie",
                column: "id_internal");

            migrationBuilder.CreateIndex(
                name: "IX_organisatie_kbo_nummer",
                schema: "integration_gtmf",
                table: "organisatie",
                column: "kbo_nummer");

            migrationBuilder.CreateIndex(
                name: "IX_organisatie_ovo_code",
                schema: "integration_gtmf",
                table: "organisatie",
                column: "ovo_code");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "meldingsobject",
                schema: "integration_gtmf");

            migrationBuilder.DropTable(
                name: "meldingsobject_statuswijziging",
                schema: "integration_gtmf");

            migrationBuilder.DropTable(
                name: "organisatie",
                schema: "integration_gtmf");

            migrationBuilder.DropTable(
                name: "projection_state",
                schema: "integration_gtmf");
        }
    }
}
