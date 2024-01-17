namespace Basisregisters.IntegrationDb.Schema.Models.Views.SuspiciousCases
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class CurrentAddressesOutsideMunicipalityBounds
    {
        public int AddressPersistentLocalId { get; set; }
        public int NisCode { get; set; }
        public DateTimeOffset Timestamp { get; set; }

        public CurrentAddressesOutsideMunicipalityBounds()
        { }
    }

    public sealed class CurrentAddressesOutsideMunicipalityBoundsConfiguration : IEntityTypeConfiguration<CurrentAddressesOutsideMunicipalityBounds>
    {
        public void Configure(EntityTypeBuilder<CurrentAddressesOutsideMunicipalityBounds> builder)
        {
            builder
                .ToView(nameof(CurrentAddressesOutsideMunicipalityBounds), SuspiciousCasesContext.Schema)
                .HasNoKey()
                .ToSqlQuery(@$"
                            SELECT
                                ""AddressPersistentLocalId"",
                                ""NisCode"",
                                ""Timestamp""
                            FROM  {ViewName} ");
        }


        public const string ViewName = @$"""{SuspiciousCasesContext.Schema}"".""VIEW_{nameof(CurrentAddressesOutsideMunicipalityBounds)}""";

        public const string Create = $@"
            CREATE MATERIALIZED VIEW IF NOT EXISTS {ViewName} AS
            SELECT
                a.""PersistentLocalId"" AS ""AddressPersistentLocalId"",
                mg.""NisCode"",
                CURRENT_TIMESTAMP AS ""Timestamp""
            FROM ""Integration"".""MunicipalityGeometries"" mg
            JOIN ""Integration"".""Addresses"" a
                ON a.""NisCode""::int = mg.""NisCode""
            WHERE ST_Within(a.""Geometry"", mg.""Geometry"") IS FALSE
            AND a.""Status"" = 'inGebruik'
            AND a.""IsRemoved"" = false;

            CREATE INDEX ""IX_{nameof(CurrentAddressesOutsideMunicipalityBounds)}_NisCode"" ON {ViewName} USING btree (""{nameof(CurrentAddressesOutsideMunicipalityBounds.NisCode)}"");
            ";
    }
}
