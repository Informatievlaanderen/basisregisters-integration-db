namespace Basisregisters.IntegrationDB.Bosa.Infrastructure
{
    using System;
    using Dapper;
    using Npgsql;

    public class DatabaseSetup
    {
        public const string Schema = "integration_bosa";
        public const string PostalCrabVersionsTable = "postal_crab_versions";
        public const string MunicipalityCrabVersionsTable = "municipality_crab_versions";
        public const string StreetNameCrabVersionsTable = "streetname_crab_versions";
        public const string AddressCrabVersionsTable = "address_crab_versions";

        private readonly string _connectionString;

        public DatabaseSetup(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void CheckIfDataPresent()
        {
            using var connection = new NpgsqlConnection(_connectionString);

            CheckTableData(connection, PostalCrabVersionsTable);
            CheckTableData(connection, MunicipalityCrabVersionsTable);
            CheckTableData(connection, StreetNameCrabVersionsTable);
            CheckTableData(connection, AddressCrabVersionsTable);
        }

        private static void CheckTableData(NpgsqlConnection connection, string table)
        {
            var postal = connection.Execute($"SELECT * FROM {Schema}.{table} LIMIT 1");
            if(postal == 0)
                throw new InvalidOperationException($"No data found in the {table} table");
        }
    }
}
