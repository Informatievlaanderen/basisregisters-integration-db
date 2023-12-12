namespace Basisregisters.IntegrationDb.Schema.Models.Views
{
    public class Views
    {
        private const string Schema = IntegrationContext.Schema;

        #region Relations

        public static class BuildingUnitAddressRelations
        {
            public const string Table = @$"""{IntegrationContext.Schema}"".""VIEW_{nameof(BuildingUnitAddressRelations)}""";

            public const string Create = $@"
            CREATE MATERIALIZED VIEW IF NOT EXISTS {Table} AS
            SELECT
                bu.""PersistentLocalId"" AS ""BuildingUnitPersistentLocalId"",
                unnested_address_id AS ""AddressPersistentLocalId"",
                CURRENT_TIMESTAMP AS ""Timestamp""
            FROM
                ""Integration"".""BuildingUnits"" bu
            WHERE
                bu.""IsRemoved"" = false
            CROSS JOIN
                unnest(string_to_array(bu.""Addresses"", ', ')::int[]) AS unnested_address_id;
            ";
        }

        public static class ParcelAddressRelations
        {
            public const string Table = @$"""{IntegrationContext.Schema}"".""VIEW_{nameof(ParcelAddressRelations)}""";

            public const string Create = $@"
            CREATE MATERIALIZED VIEW IF NOT EXISTS {Table} AS
            SELECT
                p.""CaPaKey"",
                unnested_address_id AS ""AddressPersistentLocalId"",
                CURRENT_TIMESTAMP AS ""Timestamp""
            FROM
                ""Integration"".""Parcels"" p
            WHERE
                p.""IsRemoved"" = false
            CROSS JOIN
                unnest(string_to_array(p.""Addresses"", ', ')::int[]) AS unnested_address_id;
            ";
        }

        #endregion Relations

        #region Address

        public static class CurrentAddressWithoutLinkedParcelOrBuildingUnit
        {
            public const string Table = @$"""{Schema}"".""VIEW_{nameof(CurrentAddressWithoutLinkedParcelOrBuildingUnit)}""";

            public const string Create = $@"
            CREATE MATERIALIZED VIEW IF NOT EXISTS {Table} AS
                SELECT
                        a.""PersistentLocalId"" AS ""AddressPersistentLocalId"",
                        a.""NisCode"",
                        CURRENT_TIMESTAMP AS ""Timestamp""

                FROM ""Integration"".""Addresses"" AS a
                WHERE EXISTS (
                    SELECT 1
                    FROM ""Integration"".""Addresses"" AS address
                    LEFT JOIN {ParcelAddressRelations.Table} AS parcelRelations
                        ON address.""PersistentLocalId"" = parcelRelations.""AddressPersistentLocalId""
                    LEFT JOIN {BuildingUnitAddressRelations.Table} AS buildingUnitRelations
                        ON address.""PersistentLocalId"" = buildingUnitRelations.""AddressPersistentLocalId""
                    WHERE address.""PersistentLocalId"" = a.""PersistentLocalId""
                        AND (parcelRelations.""AddressPersistentLocalId"" IS NULL AND buildingUnitRelations.""AddressPersistentLocalId"" IS NULL)
                        AND address.""Status"" ILIKE 'inGebruik'
                        AND address.""IsRemoved"" = false
                        AND address.""PositionSpecification"" = 'ligplaats'
                        AND address.""IsRemoved"" = false
                )
            ORDER BY a.""PersistentLocalId""
            ";
        }

        public static class ProposedAddressWithoutLinkedParcelOrBuildingUnit
        {
            public const string Table = @$"""{Schema}"".""VIEW_{nameof(ProposedAddressWithoutLinkedParcelOrBuildingUnit)}""";

            public const string Create = $@"
            CREATE MATERIALIZED VIEW IF NOT EXISTS {Table} AS
                SELECT
                        a.""PersistentLocalId"" AS ""AddressPersistentLocalId"",
                        a.""NisCode"",
                        CURRENT_TIMESTAMP AS ""Timestamp""

                FROM ""Integration"".""Addresses"" AS a
                WHERE EXISTS (
                    SELECT 1
                    FROM ""Integration"".""Addresses"" AS address
                    LEFT JOIN {ParcelAddressRelations.Table} AS parcelRelations
                        ON address.""PersistentLocalId"" = parcelRelations.""AddressPersistentLocalId""
                    LEFT JOIN {BuildingUnitAddressRelations.Table} AS buildingUnitRelations
                        ON address.""PersistentLocalId"" = buildingUnitRelations.""AddressPersistentLocalId""
                    WHERE address.""PersistentLocalId"" = a.""PersistentLocalId""
                        AND (parcelRelations.""AddressPersistentLocalId"" IS NULL AND buildingUnitRelations.""AddressPersistentLocalId"" IS NULL)
                        AND address.""Status"" LIKE 'voorgesteld'
                        AND address.""IsRemoved"" = false
                        AND address.""PositionSpecification"" = 'ligplaats'
                )
            ORDER BY a.""PersistentLocalId""
            ";
        }

        public static class AddressesWithoutPostalCode
        {
            public const string Table = @$"""{Schema}"".""VIEW_{nameof(AddressesWithoutPostalCode)}""";

            public const string Create = $@"
            CREATE MATERIALIZED VIEW IF NOT EXISTS {Table} AS
            SELECT
                ""PersistentLocalId"" AS ""AddressPersistentLocalId"",
                ""NisCode"",
                 CURRENT_TIMESTAMP AS ""Timestamp""

            FROM ""Integration"".""Addresses""
            WHERE ""PostalCode"" = '' OR ""PostalCode"" IS NULL
            AND ""IsRemoved"" = false
        ";
        }

        public static class CurrentAddressesOutsideMunicipalityBounds
        {
            public const string Table = @$"""{Schema}"".""VIEW_{nameof(CurrentAddressesOutsideMunicipalityBounds)}""";

            public const string Create = $@"
            CREATE MATERIALIZED VIEW IF NOT EXISTS {Table} AS
            SELECT
                a.""PersistentLocalId"" AS ""AddressPersistentLocalId"",
                mg.""NisCode"",
                CURRENT_TIMESTAMP AS ""Timestamp""
            FROM ""Integration"".""MunicipalityGeometries"" mg
            JOIN ""Integration"".""Addresses"" a
                ON a.""NisCode""::int = mg.""NisCode""
            WHERE ST_Within(a.""Geometry"", mg.""Geometry"") IS FALSE
            AND a.""Status"" = 'inGebruik'
            AND a.""IsRemoved"" = false
            ";
        }

        public static class AddressesLinkedToMultipleBuildingUnits
        {
            public const string Table = @$"""{Schema}"".""VIEW_{nameof(AddressesLinkedToMultipleBuildingUnits)}""";

            public const string Create = $@"
            CREATE MATERIALIZED VIEW IF NOT EXISTS {Table} AS
            SELECT
                a.""PersistentLocalId"" AS ""AddressPersistentLocalId"",
                COUNT(*) AS ""LinkedBuildingUnitCount"",
                a.""NisCode"",
                CURRENT_TIMESTAMP AS ""Timestamp""

            FROM {BuildingUnitAddressRelations.Table} relation
            JOIN ""Integration"".""Addresses"" a ON a.""PersistentLocalId"" = relation.""AddressPersistentLocalId""
            WHERE
                a.""IsRemoved"" = false
            GROUP BY
                a.""PersistentLocalId""
            HAVING
                COUNT(*) > 1;
            ";
        }

        #endregion Address

        #region StreetName

        public static class CurrentStreetNameWithoutLinkedRoadSegments
        {
            public const string Table = @$"""{Schema}"".""VIEW_{nameof(CurrentStreetNameWithoutLinkedRoadSegments)}""";

            public const string Create = $@"
            CREATE MATERIALIZED VIEW IF NOT EXISTS {Table} AS
            SELECT
                streetName.""PersistentLocalId"" AS ""StreetNamePersistentLocalId"",
                streetName.""NisCode"",
                CURRENT_TIMESTAMP AS ""Timestamp""
            FROM ""Integration"".""StreetNames"" AS streetName
            WHERE NOT EXISTS (
                SELECT 1
                FROM ""Integration"".""RoadSegments"" roadsegment
                WHERE roadsegment.""LeftSideStreetNameId"" = streetname.""PersistentLocalId""
                AND roadsegment.""RightSideStreetNameId"" = streetname.""PersistentLocalId""
            )
            AND streetName.""Status"" ILIKE 'ingebruik'
            AND streetName.""IsRemoved"" = false
            ";
        }

        #endregion

        #region RoadSegment

        #endregion

        #region Building

        #endregion Building

        #region BuildingUnit

        #endregion BuildingUnit

        // TODO: review
        public static class AddressesLinkedToMultipleParcels
        {
            public const string Table = @$"""{Schema}"".""VIEW_{nameof(AddressesLinkedToMultipleParcels)}""";

            public const string Create = $@"
        CREATE MATERIALIZED VIEW IF NOT EXISTS {Table} AS
        SELECT
            a.""PersistentLocalId"" AS ""AddressPersistentLocalId"",
            COUNT(*) AS ""LinkedBuildingUnitCount"",
            a.""NisCode"",
            CURRENT_TIMESTAMP AS ""Timestamp""

        FROM {ParcelAddressRelations.Table} relation
        JOIN ""Integration"".""Addresses"" a ON a.""PersistentLocalId"" = relation.""AddressPersistentLocalId""
        WHERE
            a.""IsRemoved"" = false
        GROUP BY
            a.""PersistentLocalId""
        HAVING
            COUNT(*) > 1;
        ";
        }

        public static class AddressesWithMultipleLinks
        {
            public const string Table = @$"""{Schema}"".""VIEW_{nameof(AddressesWithMultipleLinks)}""";

            public const string Create = $@"
            CREATE MATERIALIZED VIEW IF NOT EXISTS {Table} AS
            SELECT
                address.""PersistentLocalId"" AS ""AddressPersistentLocalId"",
                address.""Status"",
                address.""IsRemoved"",
                buLinks.""LinkedBuildingUnitCount"",
                parcelLinks.""LinkedParcelCount"",
                address.""NisCode"",
                CURRENT_TIMESTAMP AS ""Timestamp""
            FROM ""Integration"".""Addresses"" address
            JOIN {AddressesLinkedToMultipleBuildingUnits.Table} buLinks
                on address.""PersistentLocalId"" = buLinks.""AddressPersistentLocalId""
            JOIN {AddressesLinkedToMultipleParcels.Table} parcelLinks
                on address.""PersistentLocalId"" = parcelLinks.""AddressPersistentLocalId""
            WHERE
                address.""IsRemoved"" = false
        ";
        }

        public static class ParcelsLinkedToMultipleAddresses
        {
            public const string Table = @$"""{Schema}"".""VIEW_{nameof(ParcelsLinkedToMultipleAddresses)}""";

            public const string Create = $@"
            CREATE MATERIALIZED VIEW IF NOT EXISTS {Table} AS
             SELECT
                p.""CaPaKey"",
                p.""Status"",
                COUNT(*) AS ""LinkedAddressCount"",
                CURRENT_TIMESTAMP AS ""Timestamp""

            FROM ""Integration"".""Parcels"" p
            JOIN {ParcelAddressRelations.Table} relation
                ON p.""CaPaKey"" = relation.""CaPaKey""
            WHERE
                p.""IsRemoved"" = false
            GROUP BY
                p.""CaPaKey""
            HAVING
                COUNT(*) > 1;
            ";
        }

        public static class CurrentAddressWithoutLinkedParcels
        {
            public const string Table = @$"""{Schema}"".""VIEW_{nameof(CurrentAddressWithoutLinkedParcels)}""";

            public const string Create = $@"
            CREATE MATERIALIZED VIEW IF NOT EXISTS {Table} AS
            SELECT
                a.""PersistentLocalId"" AS ""AddressPersistentLocalId"",
                a.""NisCode"",
                CURRENT_TIMESTAMP AS ""Timestamp""

            FROM ""Integration"".""Addresses"" AS a
            WHERE EXISTS (
                SELECT 1
                FROM ""Integration"".""Addresses"" AS address
                LEFT JOIN ""Integration"".""VIEW_ParcelAddressRelations"" AS parcelRelations
                    ON address.""PersistentLocalId"" = parcelRelations.""AddressPersistentLocalId""
                WHERE address.""PersistentLocalId"" = a.""PersistentLocalId""
                    AND parcelRelations.""AddressPersistentLocalId"" IS NULL
                    AND address.""Status"" ILIKE 'inGebruik'
                    AND address.""IsRemoved"" = false
            )
            ORDER BY a.""PersistentLocalId""
        ";
        }
    }
}
