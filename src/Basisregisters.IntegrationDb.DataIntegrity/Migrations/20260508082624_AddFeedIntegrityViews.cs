using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Basisregisters.IntegrationDb.DataIntegrity.Migrations
{
    using Feeds;

    /// <inheritdoc />
    public partial class AddFeedIntegrityViews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.EnsureSchema(
                name: DataIntegrityContext.Schema);

            migrationBuilder.Sql(MunicipalityViewRepository.DropAndCreateMunicipalityViewSql);
            migrationBuilder.Sql(PostalViewRepository.DropAndCreatePostalViewSql);
            migrationBuilder.Sql(PostalNameViewRepository.DropAndCreatePostalNameViewSql);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"DROP MATERIALIZED VIEW IF EXISTS {DataIntegrityContext.Schema}.municipality_feed_latest_integrity");
            migrationBuilder.Sql($"DROP MATERIALIZED VIEW IF EXISTS {DataIntegrityContext.Schema}.postal_feed_latest_integrity");
        }
    }
}
