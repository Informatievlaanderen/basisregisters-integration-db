namespace Basisregisters.IntegrationDb.Schema.Models.Views
{
    public class Views
    {
        public const string Schema = "SuspiciousCases";

        public static class BuildingUnitAddressRelations
        {
            public const string Table = @$"""{IntegrationContext.Schema}"".""VIEW_{nameof(BuildingUnitAddressRelations)}""";
            public const string Create  = $@"
            CREATE MATERIALIZED VIEW IF NOT EXISTS {Table} AS
            SELECT
                bu.""PersistentLocalId"" AS ""BuildingUnitPersistentLocalId"",
                unnested_address_id AS ""AddressPersistentLocalId"",
                CURRENT_TIMESTAMP AS ""Timestamp""
            FROM
                ""Integration"".""BuildingUnits"" bu
            CROSS JOIN
                unnest(string_to_array(bu.""Addresses"", ', ')::int[]) AS unnested_address_id;
            ";
        }

        // public const string VIEW_BuildingUnitAddressRelations = $@"
        // CREATE MATERIALIZED VIEW IF NOT EXISTS ""{Schema}"".""{nameof(VIEW_BuildingUnitAddressRelations)}"" AS
        // SELECT
        //     bu.""PersistentLocalId"" AS ""BuildingUnitPersistentLocalId"",
        //     unnested_address_id AS ""AddressPersistentLocalId"",
        //     CURRENT_TIMESTAMP AS ""Timestamp""
        // FROM
        //     ""Integration"".""BuildingUnits"" bu
        // CROSS JOIN
        //     unnest(string_to_array(bu.""Addresses"", ', ')::int[]) AS unnested_address_id;
        // ";


        public static class ParcelAddressRelations
        {
            public const string Table = @$"""{IntegrationContext.Schema}"".""VIEW_{nameof(ParcelAddressRelations)}""";
            public const string Create  = $@"
            CREATE MATERIALIZED VIEW IF NOT EXISTS {Table} AS
            SELECT
                p.""CaPaKey"",
                unnested_address_id AS ""AddressPersistentLocalId"",
                CURRENT_TIMESTAMP AS ""Timestamp""
            FROM
                ""Integration"".""Parcels"" p
            CROSS JOIN
                unnest(string_to_array(p.""Addresses"", ', ')::int[]) AS unnested_address_id;
            ";
        }

        // public const string VIEW_ParcelAddressRelations = $@"
        // CREATE MATERIALIZED VIEW IF NOT EXISTS ""{Schema}"".""{nameof(VIEW_ParcelAddressRelations)}"" AS
        // SELECT
        //     p.""CaPaKey"",
        //     unnested_address_id AS ""AddressPersistentLocalId"",
        //     CURRENT_TIMESTAMP AS ""Timestamp""
        // FROM
        //     ""Integration"".""Parcels"" p
        // CROSS JOIN
        //     unnest(string_to_array(p.""Addresses"", ', ')::int[]) AS unnested_address_id;
        // ";

        public static class AddressesLinkedToMultipleBuildingUnits
        {
            public const string Table = @$"""{Schema}"".""VIEW_{nameof(AddressesLinkedToMultipleBuildingUnits)}""";
            public const string Create  = $@"
            CREATE MATERIALIZED VIEW IF NOT EXISTS {Table} AS
            SELECT
                a.""PersistentLocalId"" AS ""AddressPersistentLocalId"",
                COUNT(*) AS ""LinkedBuildingUnitCount"",
                a.""NisCode"",
                CURRENT_TIMESTAMP AS ""Timestamp""

            FROM {BuildingUnitAddressRelations.Table} relation
            JOIN ""Integration"".""Addresses"" a ON a.""PersistentLocalId"" = relation.""AddressPersistentLocalId""
            GROUP BY
                a.""PersistentLocalId""
            HAVING
                COUNT(*) > 1;
            ";
        }

//         public static class AddressesLinkedToMultipleBuildingUnits
//         {
//             public const string Table = @$"""{Schema}"".""{nameof(AddressesLinkedToMultipleBuildingUnits)}""";
//             public const string Create =  @$"
//             CREATE MATERIALIZED VIEW IF NOT EXISTS {Table} AS
//             SELECT
//                 a.""PersistentLocalId"" AS ""AddressPersistentLocalId"",
//                 COUNT(*) AS ""LinkedBuildingUnitCount"",
//                 a.""NisCode"",
//                 CURRENT_TIMESTAMP AS ""Timestamp""
//
//             FROM {BuildingUnitAddressRelations.Table} relation
//             JOIN ""Integration"".""Addresses"" a ON a.""PersistentLocalId"" = relation.""AddressPersistentLocalId""
//             GROUP BY
//                 a.""PersistentLocalId""
//             HAVING
//                 COUNT(*) > 1;
//             ";
 //       }

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
        GROUP BY
            a.""PersistentLocalId""
        HAVING
            COUNT(*) > 1;
        ";
    }

//         public const string VIEW_AddressesLinkedToMultipleParcels = $@"
//         CREATE MATERIALIZED VIEW IF NOT EXISTS ""{Schema}"".""{nameof(VIEW_AddressesLinkedToMultipleParcels)}"" AS
//         SELECT
//             a.""PersistentLocalId"" AS ""AddressPersistentLocalId"",
//             COUNT(*) AS ""LinkedParcelCount"",
//             a.""NisCode"",
//             CURRENT_TIMESTAMP AS ""Timestamp""
//
//         FROM ""Integration"".""VIEW_ParcelAddressRelations"" relation
//         JOIN ""Integration"".""Addresses"" a ON a.""PersistentLocalId"" = relation.""AddressPersistentLocalId""
//         GROUP BY
//             a.""PersistentLocalId""
//         HAVING
//             COUNT(*) > 1;
//         ";

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
        ";
        }

        // public const string VIEW_AddressesWithMultipleLinks = $@"
        // CREATE MATERIALIZED VIEW IF NOT EXISTS ""{Schema}"".""{nameof(VIEW_AddressesWithMultipleLinks)}"" AS
        // SELECT
        //     address.""PersistentLocalId"" AS ""AddressPersistentLocalId"",
        //     address.""Status"",
        //     address.""IsRemoved"",
        //     buLinks.""LinkedBuildingUnitCount"",
        //     parcelLinks.""LinkedParcelCount"",
        //     address.""NisCode"",
        //     CURRENT_TIMESTAMP AS ""Timestamp""
        // FROM ""Integration"".""Addresses"" address
        // JOIN ""Integration"".""VIEW_AddressesLinkedToMultipleBuildingUnits"" buLinks
        //     on address.""PersistentLocalId"" = buLinks.""AddressPersistentLocalId""
        // JOIN ""Integration"".""VIEW_AddressesLinkedToMultipleParcels"" parcelLinks
        //     on address.""PersistentLocalId"" = parcelLinks.""AddressPersistentLocalId""
        // ";

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
            GROUP BY
                p.""CaPaKey""
            HAVING
                COUNT(*) > 1;
            ";
        }

//         public const string VIEW_ParcelsLinkedToMultipleAddresses = $@"
//         CREATE MATERIALIZED VIEW IF NOT EXISTS ""{Schema}"".""{nameof(VIEW_ParcelsLinkedToMultipleAddresses)}"" AS
//          SELECT
//             p.""CaPaKey"",
//             p.""Status"",
//             COUNT(*) AS ""LinkedAddressCount"",
//             CURRENT_TIMESTAMP AS ""Timestamp""
//
//         FROM ""Integration"".""Parcels"" p
//         JOIN ""Integration"".""VIEW_ParcelAddressRelations"" relation
//             ON p.""CaPaKey"" = relation.""CaPaKey""
//         GROUP BY
//             p.""CaPaKey""
//         HAVING
//             COUNT(*) > 1;
//         ";

        public static class CurrentAddressWithoutLinkedParcels
        {
            public const string Table = @$"""{Schema}"".""VIEW_{nameof(CurrentAddressWithoutLinkedParcels)}""";
            public const string Create = $@"
            CREATE MATERIALIZED VIEW IF NOT EXISTS {Table} AS
            SELECT
                a.""PersistentLocalId"" AS ""AddressPersistentLocalId"",
                a.""NisCode""::int,
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

//         public const string VIEW_CurrentAddressWithoutLinkedParcels = $@"
//         CREATE MATERIALIZED VIEW IF NOT EXISTS ""{Schema}"".""{nameof(VIEW_CurrentAddressWithoutLinkedParcels)}"" AS
//         SELECT
//             a.""PersistentLocalId"" AS ""AddressPersistentLocalId"",
//             a.""NisCode""::int,
//             CURRENT_TIMESTAMP AS ""Timestamp""
//
//         FROM ""Integration"".""Addresses"" AS a
//         WHERE EXISTS (
//             SELECT 1
//             FROM ""Integration"".""Addresses"" AS address
//             LEFT JOIN ""Integration"".""VIEW_ParcelAddressRelations"" AS parcelRelations
//                 ON address.""PersistentLocalId"" = parcelRelations.""AddressPersistentLocalId""
//             WHERE address.""PersistentLocalId"" = a.""PersistentLocalId""
//                 AND parcelRelations.""AddressPersistentLocalId"" IS NULL
//                 AND address.""Status"" ILIKE 'inGebruik'
//                 AND address.""IsRemoved"" = false
//         )
//         ORDER BY a.""PersistentLocalId""
//         ";

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
                )
            ORDER BY a.""PersistentLocalId""
            ";
        }

//         public const string VIEW_CurrentAddressWithoutLinkedParcelOrBuildingUnit = $@"
//         CREATE MATERIALIZED VIEW IF NOT EXISTS ""{Schema}"".""{nameof(VIEW_CurrentAddressWithoutLinkedParcelOrBuildingUnit)}"" AS
//         SELECT
//                 a.""PersistentLocalId"" AS ""AddressPersistentLocalId"",
//                 a.""NisCode"",
//                 CURRENT_TIMESTAMP AS ""Timestamp""
//
//         FROM ""Integration"".""Addresses"" AS a
//         WHERE EXISTS (
//             SELECT 1
//             FROM ""Integration"".""Addresses"" AS address
//             LEFT JOIN ""Integration"".""VIEW_ParcelAddressRelations"" AS parcelRelations
//                 ON address.""PersistentLocalId"" = parcelRelations.""AddressPersistentLocalId""
//             LEFT JOIN ""Integration"".""VIEW_BuildingUnitAddressRelations"" AS buildingUnitRelations
//                 ON address.""PersistentLocalId"" = buildingUnitRelations.""AddressPersistentLocalId""
//             WHERE address.""PersistentLocalId"" = a.""PersistentLocalId""
//                 AND (parcelRelations.""AddressPersistentLocalId"" IS NULL AND buildingUnitRelations.""AddressPersistentLocalId"" IS NULL)
//                 AND address.""Status"" ILIKE 'inGebruik'
//                 AND address.""IsRemoved"" = false
//         )
//         ORDER BY a.""PersistentLocalId""
//         ";

        public static class CurrentStreetnameWithoutLinkedRoadSegments
        {
            public const string Table = @$"""{Schema}"".""VIEW_{nameof(CurrentStreetnameWithoutLinkedRoadSegments)}""";
            public const string Create = $@"
            CREATE MATERIALIZED VIEW IF NOT EXISTS {Table} AS
            SELECT
                streetname.""PersistentLocalId"" AS ""StreetNamePersistentLocalId"",
                streetname.""NisCode"",
                CURRENT_TIMESTAMP AS ""Timestamp""
            FROM ""Integration"".""StreetNames"" streetnamefr
            WHERE NOT EXISTS (
                SELECT 1
                FROM ""Integration"".""RoadSegments"" roadsegment
                WHERE roadsegment.""LeftSideStreetNameId"" = streetname.""PersistentLocalId""
                AND roadsegment.""RightSideStreetNameId"" = streetname.""PersistentLocalId""
            )
            AND streetname.""Status"" ILIKE 'ingebruik'
            AND streetname.""IsRemoved"" = false
        ";
        }

        // public const string VIEW_CurrentStreetnameWithoutLinkedRoadSegments = $@"
        // CREATE MATERIALIZED VIEW IF NOT EXISTS ""{Schema}"".""{nameof(VIEW_CurrentStreetnameWithoutLinkedRoadSegments)}"" AS
        // SELECT
        //     streetname.""PersistentLocalId"" AS ""StreetNamePersistentLocalId"",
        //     streetname.""NisCode"",
        //     CURRENT_TIMESTAMP AS ""Timestamp""
        // FROM ""Integration"".""StreetNames"" streetnamefr
        // WHERE NOT EXISTS (
        //     SELECT 1
        //     FROM ""Integration"".""RoadSegments"" roadsegment
        //     WHERE roadsegment.""LeftSideStreetNameId"" = streetname.""PersistentLocalId""
        //     AND roadsegment.""RightSideStreetNameId"" = streetname.""PersistentLocalId""
        // )
        // AND streetname.""Status"" ILIKE 'ingebruik'
        // AND streetname.""IsRemoved"" = false
        // ";

        public static class AddressesWithoutPostalCode
        {
            public const string Table = @$"""{Schema}"".""VIEW_{nameof(AddressesWithoutPostalCode)}""";
            public const string Create = $@"
            CREATE MATERIALIZED VIEW IF NOT EXISTS {Table} AS
            SELECT
                ""PersistentLocalId"" AS ""AddressPersistentLocalId"",
                ""NisCode""::int,
                 CURRENT_TIMESTAMP AS ""Timestamp""

            FROM ""Integration"".""Addresses""
            WHERE ""PostalCode"" = '' OR ""PostalCode"" IS NULL
        ";
        }

//         public const string VIEW_AddressesWithoutPostalCode = $@"
//         CREATE MATERIALIZED VIEW IF NOT EXISTS ""{Schema}"".""{nameof(VIEW_AddressesWithoutPostalCode)}"" AS
//         SELECT
//             ""PersistentLocalId"" AS ""AddressPersistentLocalId"",
//             ""NisCode""::int,
//              CURRENT_TIMESTAMP AS ""Timestamp""
//
//         FROM ""Integration"".""Addresses""
//         WHERE ""PostalCode"" = '' OR ""PostalCode"" IS NULL
//         ";

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

        // public const string VIEW_CurrentAddressesOutsideMunicipalityBounds = $@"
        // CREATE MATERIALIZED VIEW IF NOT EXISTS ""{Schema}"".""{nameof(VIEW_CurrentAddressesOutsideMunicipalityBounds)}"" AS
        // SELECT
        //     a.""PersistentLocalId"" AS ""AddressPersistentLocalId"",
        //     mg.""NisCode"",
        //     CURRENT_TIMESTAMP AS ""Timestamp""
        // FROM ""Integration"".""MunicipalityGeometries"" mg
        // JOIN ""Integration"".""Addresses"" a
        //     ON a.""NisCode""::int = mg.""NisCode""
        // WHERE ST_Within(a.""Geometry"", mg.""Geometry"") IS FALSE
        // AND a.""Status"" = 'inGebruik'
        // AND a.""IsRemoved"" = false
        // ";
    }
}
