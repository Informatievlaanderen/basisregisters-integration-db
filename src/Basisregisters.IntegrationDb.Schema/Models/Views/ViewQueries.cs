namespace Basisregisters.IntegrationDb.Schema.Models.Views
{
    public class ViewQueries
    {
        public const string VIEW_BuildingUnitAddressRelations = $@"
        CREATE MATERIALIZED VIEW IF NOT EXISTS ""Integration"".""{nameof(VIEW_BuildingUnitAddressRelations)}"" AS
        SELECT
            bu.""PersistentLocalId"" AS ""BuildingUnitPersistentLocalId"",
            unnested_address_id AS ""AddressPersistentLocalId"",
            CURRENT_TIMESTAMP AS ""Timestamp""
        FROM
            ""Integration"".""BuildingUnits"" bu
        CROSS JOIN
            unnest(string_to_array(bu.""Addresses"", ', ')::int[]) AS unnested_address_id;
        ";

        public const string VIEW_ParcelAddressRelations = $@"
        CREATE MATERIALIZED VIEW IF NOT EXISTS ""Integration"".""{nameof(VIEW_ParcelAddressRelations)}"" AS
        SELECT
            p.""CaPaKey"",
            unnested_address_id AS ""AddressPersistentLocalId"",
            CURRENT_TIMESTAMP AS ""Timestamp""
        FROM
            ""Integration"".""Parcels"" p
        CROSS JOIN
            unnest(string_to_array(p.""Addresses"", ', ')::int[]) AS unnested_address_id;
        ";

        public const string VIEW_AddressesLinkedToMultipleBuildingUnits = @$"
        CREATE MATERIALIZED VIEW IF NOT EXISTS ""Integration"".""{nameof(VIEW_AddressesLinkedToMultipleBuildingUnits)}"" AS
        SELECT
            a.""PersistentLocalId"" AS ""AddressPersistentLocalId"",
            COUNT(*) AS ""LinkedBuildingUnitCount"",
            a.""NisCode"",
            CURRENT_TIMESTAMP AS ""Timestamp""

        FROM ""Integration"".""VIEW_BuildingUnitAddressRelations"" relation
        JOIN ""Integration"".""Addresses"" a ON a.""PersistentLocalId"" = relation.""AddressPersistentLocalId""
        GROUP BY
            a.""PersistentLocalId""
        HAVING
            COUNT(*) > 1;
        ";

        public const string VIEW_AddressesLinkedToMultipleParcels = $@"
        CREATE MATERIALIZED VIEW IF NOT EXISTS ""Integration"".""{nameof(VIEW_AddressesLinkedToMultipleParcels)}"" AS
        SELECT
            a.""PersistentLocalId"" AS ""AddressPersistentLocalId"",
            COUNT(*) AS ""LinkedParcelCount"",
            a.""NisCode"",
            CURRENT_TIMESTAMP AS ""Timestamp""

        FROM ""Integration"".""VIEW_ParcelAddressRelations"" relation
        JOIN ""Integration"".""Addresses"" a ON a.""PersistentLocalId"" = relation.""AddressPersistentLocalId""
        GROUP BY
            a.""PersistentLocalId""
        HAVING
            COUNT(*) > 1;
        ";

        public const string VIEW_AddressesWithMultipleLinks = $@"
        CREATE MATERIALIZED VIEW IF NOT EXISTS ""Integration"".""{nameof(VIEW_AddressesWithMultipleLinks)}"" AS
        SELECT
            address.""PersistentLocalId"" AS ""AddressPersistentLocalId"",
            address.""Status"",
            address.""IsRemoved"",
            buLinks.""LinkedBuildingUnitCount"",
            parcelLinks.""LinkedParcelCount"",
            address.""NisCode"",
            CURRENT_TIMESTAMP AS ""Timestamp""
        FROM ""Integration"".""Addresses"" address
        JOIN ""Integration"".""VIEW_AddressesLinkedToMultipleBuildingUnits"" buLinks
            on address.""PersistentLocalId"" = buLinks.""AddressPersistentLocalId""
        JOIN ""Integration"".""VIEW_AddressesLinkedToMultipleParcels"" parcelLinks
            on address.""PersistentLocalId"" = parcelLinks.""AddressPersistentLocalId""
        ";

        public const string VIEW_ParcelsLinkedToMultipleAddresses = $@"
        CREATE MATERIALIZED VIEW IF NOT EXISTS ""Integration"".""{nameof(VIEW_ParcelsLinkedToMultipleAddresses)}"" AS
         SELECT
            p.""CaPaKey"",
            p.""Status"",
            COUNT(*) AS ""LinkedAddressCount"",
            CURRENT_TIMESTAMP AS ""Timestamp""

        FROM ""Integration"".""Parcels"" p
        JOIN ""Integration"".""VIEW_ParcelAddressRelations"" relation
            ON p.""CaPaKey"" = relation.""CaPaKey""
        GROUP BY
            p.""CaPaKey""
        HAVING
            COUNT(*) > 1;
        ";

        public const string VIEW_ActiveAddressWithoutLinkedParcels = $@"
        CREATE MATERIALIZED VIEW IF NOT EXISTS ""Integration"".""{nameof(VIEW_ActiveAddressWithoutLinkedParcels)}"" AS
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

        public const string VIEW_ActiveAddressWithoutLinkedParcelOrBuildingUnit = $@"
        CREATE MATERIALIZED VIEW IF NOT EXISTS ""Integration"".""{nameof(VIEW_ActiveAddressWithoutLinkedParcelOrBuildingUnit)}"" AS
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
            LEFT JOIN ""Integration"".""VIEW_BuildingUnitAddressRelations"" AS buildingUnitRelations
                ON address.""PersistentLocalId"" = buildingUnitRelations.""AddressPersistentLocalId""
            WHERE address.""PersistentLocalId"" = a.""PersistentLocalId""
                AND (parcelRelations.""AddressPersistentLocalId"" IS NULL AND buildingUnitRelations.""AddressPersistentLocalId"" IS NULL)
                AND address.""Status"" ILIKE 'inGebruik'
                AND address.""IsRemoved"" = false
        )
        ORDER BY a.""PersistentLocalId""
        ";

        public const string VIEW_ActiveStreetnameWithoutLinkedRoadSegments = $@"
        CREATE MATERIALIZED VIEW IF NOT EXISTS ""Integration"".""{nameof(VIEW_ActiveStreetnameWithoutLinkedRoadSegments)}"" AS
        SELECT
            streetname.""PersistentLocalId"" AS ""StreetNamePersistentLocalId"",
            streetname.""NisCode"",
            CURRENT_TIMESTAMP AS ""Timestamp""
        FROM ""Integration"".""StreetNames"" streetname
        WHERE NOT EXISTS (
            SELECT 1
            FROM ""Integration"".""RoadSegments"" roadsegment
            WHERE roadsegment.""LeftSideStreetNameId"" = streetname.""PersistentLocalId""
            AND roadsegment.""RightSideStreetNameId"" = streetname.""PersistentLocalId""
        )
        AND streetname.""Status"" ILIKE 'ingebruik'
        AND streetname.""IsRemoved"" = false
        ";

        public const string VIEW_AddressesWithoutPostalCode = $@"
        CREATE MATERIALIZED VIEW IF NOT EXISTS ""Integration"".""{nameof(VIEW_AddressesWithoutPostalCode)}"" AS
        SELECT
            ""PersistentLocalId"" AS ""AddressPersistentLocalId"",
            ""NisCode""::int,
             CURRENT_TIMESTAMP AS ""Timestamp""

        FROM ""Integration"".""Addresses""
        WHERE ""PostalCode"" = '' OR ""PostalCode"" IS NULL
        ";
    }
}
