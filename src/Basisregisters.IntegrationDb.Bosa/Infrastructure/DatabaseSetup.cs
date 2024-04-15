namespace Basisregisters.IntegrationDB.Bosa.Infrastructure
{
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

        public void CreateIfNotExist()
        {
            using var connection = new NpgsqlConnection(_connectionString);

            connection.Execute("CREATE SCHEMA IF NOT EXISTS integration_bosa");
            connection.Execute($"CREATE TABLE IF NOT EXISTS {Schema}.{PostalCrabVersionsTable} (postal_code varchar(4) PRIMARY KEY, version_timestamp TEXT NOT NULL)");
            connection.Execute($"CREATE TABLE IF NOT EXISTS {Schema}.{MunicipalityCrabVersionsTable} (nis_code varchar(5) PRIMARY KEY, version_timestamp TEXT NOT NULL)");
            connection.Execute($"CREATE TABLE IF NOT EXISTS {Schema}.{StreetNameCrabVersionsTable} (streetname_persistent_local_id INT PRIMARY KEY, version_timestamp TEXT NOT NULL, created_on TEXT NOT NULL)");
            connection.Execute($"CREATE TABLE IF NOT EXISTS {Schema}.{AddressCrabVersionsTable} (address_persistent_local_id INT PRIMARY KEY, version_timestamp TEXT NOT NULL, created_on TEXT NOT NULL)");
        }
    }
}
