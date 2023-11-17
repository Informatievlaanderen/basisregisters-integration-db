namespace Basisregisters.IntegrationDb.Schema.Models.Views
{
    public class ViewQueries
    {
        public const string VIEW_AddressesLinkedToMultipleBuildingUnits = @$"
            CREATE MATERIALIZED VIEW ""Integration"".""{nameof(VIEW_AddressesLinkedToMultipleBuildingUnits)}"" AS
            SELECT a.""FullName"", b.""PersistentLocalId"", b.""Status""
            FROM ""Integration"".""BuildingUnits"" b
                INNER JOIN ""Integration"".""Addresses"" a ON a.""PersistentLocalId""::TEXT = b.""Addresses""
            WHERE b.""Addresses"" IN (
                SELECT REPLACE(value, ' ', '') AS Adres_ObjectID
                FROM (
                    SELECT unnest(string_to_array(""Addresses"", ',')) AS value
            FROM ""Integration"".""BuildingUnits""
            WHERE ""Status"" <> 'gehistoreerd' AND ""Addresses"" <> ''
            GROUP BY ""Addresses""
            HAVING count(""Addresses"") > 1
                ) a
                WHERE rtrim(value) <> ''
                )
            ORDER BY b.""Addresses"";
        ";

        public const string VIEW_ParcelsLinkedToMultipleHouseNumbers = $@"
            CREATE MATERIALIZED VIEW ""Integration"".""{nameof(VIEW_ParcelsLinkedToMultipleHouseNumbers)}"" AS
            SELECT a.""FullName"", p.""CaPaKey"", p.""Addresses""
            FROM ""Integration"".""Parcels"" p
            INNER JOIN ""Integration"".""Addresses"" a ON a.""PersistentLocalId""::TEXT = p.""Addresses""
            WHERE p.""Addresses"" IN (
                SELECT REPLACE(value, ' ', '') AS Adres_ObjectID
                FROM (
                    SELECT ""Addresses""
                    FROM ""Integration"".""Parcels""
                    WHERE ""Status"" = 'gerealiseerd' AND ""Addresses"" <> ''
                    GROUP BY ""Addresses""
                    HAVING count(""Addresses"") > 1
                ) a
                CROSS JOIN LATERAL unnest(string_to_array(""Addresses"", ',')) AS value
                WHERE rtrim(value) <> ''
            )
            ORDER BY p.""Addresses"";

            -- Second query
            SELECT REPLACE(value, ' ', '') AS Adres_ObjectID
            FROM ""Integration"".""Parcels""
            CROSS JOIN LATERAL unnest(string_to_array(""Addresses"", ',')) AS value
            WHERE rtrim(value) <> '';
        ";

        public const string VIEW_ActiveHouseNumberWithoutLinkedParcel = @$"
            CREATE MATERIALIZED VIEW ""Integration"".""{nameof(VIEW_ActiveHouseNumberWithoutLinkedParcel)}"" AS
            SELECT *
            FROM ""Integration"".""Addresses""
            WHERE ""Status"" = 'ingebruik'
              AND ""PersistentLocalId""::TEXT NOT IN (
                SELECT REPLACE(value, ' ', '') AS Address_ObjectID
                FROM ""Integration"".""Parcels""
                CROSS JOIN LATERAL unnest(string_to_array(""Addresses"", ',')) AS value
                WHERE rtrim(value) <> ''
                  AND ""Status"" = 'gerealiseerd'
                  AND ""IsRemoved"" = false
              );

            SELECT *
            FROM ""Integration"".""Addresses""
            WHERE ""Status"" = 'ingebruik'
              AND ""PersistentLocalId""::TEXT NOT IN (
                SELECT REPLACE(value, ' ', '') AS Address_ObjectID
                FROM ""Integration"".""Parcels""
                CROSS JOIN LATERAL unnest(string_to_array(""Addresses"", ',')) AS value
                WHERE rtrim(value) <> ''
                  AND ""Status"" = 'gerealiseerd'
                  AND ""IsRemoved"" = false
              )
              AND ""PositionSpecification"" = 'perceel';

            SELECT *
            FROM ""Integration"".""Parcels""
            WHERE ""Addresses"" LIKE '%1274940%';
        ";

        public const string VIEW_BuildingUnitAddressRelations = $@"
            CREATE MATERIALIZED VIEW ""Integration"".""{nameof(VIEW_BuildingUnitAddressRelations)}"" AS
            SELECT
                bu.""PersistentLocalId"",
                unnested_address_id AS AddressId
            FROM
                ""Integration"".""BuildingUnits"" bu
            CROSS JOIN
                unnest(string_to_array(bu.""Addresses"", ', ')::int[]) AS unnested_address_id;
        ";

        public const string VIEW_ParcelAddressRelations = $@"
            CREATE MATERIALIZED VIEW ""Integration"".""{nameof(VIEW_ParcelAddressRelations)}"" AS
            SELECT
                p.""CaPaKey"",
                unnested_address_id AS AddressId
            FROM
                ""Integration"".""Parcels"" p
            CROSS JOIN
                unnest(string_to_array(p.""Addresses"", ', ')::int[]) AS unnested_address_id;
        ";
    }
}
