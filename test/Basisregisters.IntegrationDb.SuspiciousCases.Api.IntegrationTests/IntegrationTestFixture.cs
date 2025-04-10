namespace Basisregisters.IntegrationDb.SuspiciousCases.Api.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.DockerUtilities;
    using Duende.IdentityModel;
    using Duende.IdentityModel.Client;
    using Infrastructure;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Npgsql;
    using SuspiciousCases.Infrastructure;
    using Xunit;

    public class IntegrationTestFixture : IAsyncLifetime
    {
        private string _clientId = string.Empty;
        private string _clientSecret = string.Empty;
        private readonly IDictionary<string, AccessToken> _accessTokens = new Dictionary<string, AccessToken>();

        public TestServer TestServer { get; private set; } = null!;
        public NpgsqlConnection NpgsqlConnection { get; private set; } = null!;

        public async Task<string> GetAccessToken(string requiredScopes)
        {
            if (_accessTokens.ContainsKey(requiredScopes) && !_accessTokens[requiredScopes].IsExpired)
            {
                return _accessTokens[requiredScopes].Token;
            }

            var tokenClient = new TokenClient(
                () => new HttpClient(),
                new TokenClientOptions
                {
                    Address = "https://authenticatie-ti.vlaanderen.be/op/v1/token",
                    ClientId = _clientId,
                    ClientSecret = _clientSecret,
                    Parameters = new Parameters(new[] { new KeyValuePair<string, string>("scope", requiredScopes) })
                });

            var response = await tokenClient.RequestTokenAsync(OidcConstants.GrantTypes.ClientCredentials);

            _accessTokens[requiredScopes] = new AccessToken(response.AccessToken!, response.ExpiresIn);

            return _accessTokens[requiredScopes].Token;
        }

        public async Task InitializeAsync()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables()
                .Build();

            _clientId = configuration.GetValue<string>("ClientId")!;
            _clientSecret = configuration.GetValue<string>("ClientSecret")!;

            using var _ = DockerComposer.Compose("postgresql.yml", "suspicious-cases-integration-tests");
            await WaitForSqlServerToBecomeAvailable();

            await CreateDatabase();

            var hostBuilder = new WebHostBuilder()
                .UseConfiguration(configuration)
                .UseStartup<Startup>()
                .ConfigureLogging(loggingBuilder => loggingBuilder.AddConsole())
                .UseTestServer();

            TestServer = new TestServer(hostBuilder);
        }

        private async Task WaitForSqlServerToBecomeAvailable()
        {
            foreach (var _ in Enumerable.Range(0, 60))
            {
                await Task.Delay(TimeSpan.FromSeconds(1));

                if (await OpenConnection())
                {
                    break;
                }
            }
        }

        private async Task<bool> OpenConnection()
        {
            try
            {
                NpgsqlConnection = new NpgsqlConnection("Host=localhost;port=5434;Username=postgres;Password=postgres");
                await NpgsqlConnection.OpenAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async Task CreateDatabase()
        {
            await using var command = new NpgsqlCommand("CREATE DATABASE integration", NpgsqlConnection);
            await command.ExecuteNonQueryAsync();

            await using var postgisExtensioncommand = new NpgsqlCommand("CREATE EXTENSION postgis", NpgsqlConnection);
            await postgisExtensioncommand.ExecuteNonQueryAsync();

            var tables = new[]
            {
                SchemaLatestItems.StreetName,
                SchemaLatestItems.Address,
                SchemaLatestItems.Building,
                SchemaLatestItems.BuildingUnit,
                SchemaLatestItems.BuildingUnitAddresses,
                SchemaLatestItems.Parcel,
                SchemaLatestItems.ParcelAddresses,
                SchemaLatestItems.Postal,
                SchemaLatestItems.RoadSegment,
                SchemaLatestItems.Municipality,
                SchemaLatestItems.MunicipalityGeometries,
            };

            foreach (var schema in tables.Select(x => x.Split('.').First()))
            {
                await using var createSchemaCommand = new NpgsqlCommand($"CREATE SCHEMA IF NOT EXISTS {schema}", NpgsqlConnection);
                await createSchemaCommand.ExecuteNonQueryAsync();
            }

            var sql =
                @$"CREATE TABLE IF NOT EXISTS {SchemaLatestItems.Municipality} (
                    building_unit_persistent_local_id INT,
                    municipality_id UUID,
                    nis_code INT,
                    name_dutch TEXT,
                    version_timestamp TIMESTAMP WITH TIME ZONE
                );";
            await using var createMunicipalityTable = new NpgsqlCommand(sql, NpgsqlConnection);
            await createMunicipalityTable.ExecuteNonQueryAsync();

            sql =
                @$"CREATE TABLE IF NOT EXISTS {SchemaLatestItems.MunicipalityGeometries} (
                    nis_code INT,
                    geometry geometry
                );";
            await using var createMunicipalityGeometriesTable = new NpgsqlCommand(sql, NpgsqlConnection);
            await createMunicipalityGeometriesTable.ExecuteNonQueryAsync();

            sql =
                @$"CREATE TABLE IF NOT EXISTS {SchemaLatestItems.StreetName} (
                    persistent_local_id INT,
                    municipality_id UUID,
                    nis_code INT,
                    name_dutch TEXT,
                    status INT,
                    is_removed BOOL,
                    version_timestamp TIMESTAMP WITH TIME ZONE
                );";
            await using var createStreetNameTable = new NpgsqlCommand(sql, NpgsqlConnection);
            await createStreetNameTable.ExecuteNonQueryAsync();

            sql =
                @$"CREATE TABLE IF NOT EXISTS {SchemaLatestItems.Address} (
                    persistent_local_id INT,
                    street_name_persistent_local_id INT,
                    house_number TEXT,
                    box_number TEXT,
                    postal_code TEXT,
                    status INT,
                    removed BOOL,
                    position_specification INT,
                    position_method INT,
                    geometry geometry,
                    version_timestamp TIMESTAMP WITH TIME ZONE
                );";
            await using var createAddressTable = new NpgsqlCommand(sql, NpgsqlConnection);
            await createAddressTable.ExecuteNonQueryAsync();

            sql =
                @$"CREATE TABLE IF NOT EXISTS {SchemaLatestItems.Building} (
                    building_persistent_local_id INT,
                    version_timestamp TIMESTAMP WITH TIME ZONE,
                    nis_code INT,
                    status TEXT,
                    is_removed BOOL,
                    geometry geometry
                );";
            await using var createBuildingTable = new NpgsqlCommand(sql, NpgsqlConnection);
            await createBuildingTable.ExecuteNonQueryAsync();

            sql =
                @$"CREATE TABLE IF NOT EXISTS {SchemaLatestItems.BuildingUnit} (
                    building_unit_persistent_local_id INT,
                    building_persistent_local_id INT,
                    version_timestamp TIMESTAMP WITH TIME ZONE,
                    is_removed BOOL,
                    geometry geometry,
                    status TEXT,
                    function TEXT
                );";
            await using var createBuildingUnitTable = new NpgsqlCommand(sql, NpgsqlConnection);
            await createBuildingUnitTable.ExecuteNonQueryAsync();

            sql =
                @$"CREATE TABLE IF NOT EXISTS {SchemaLatestItems.BuildingUnitAddresses} (
                    building_unit_persistent_local_id INT,
                    address_persistent_local_id INT
                );";
            await using var createBuildingUnitAddressTable = new NpgsqlCommand(sql, NpgsqlConnection);
            await createBuildingUnitAddressTable.ExecuteNonQueryAsync();

            sql =
                @$"CREATE TABLE IF NOT EXISTS {SchemaLatestItems.ParcelAddresses} (
                    address_persistent_local_id INT,
                    capakey text,
                    parcel_id uuid
                );";
            await using var createParcelAddressTable = new NpgsqlCommand(sql, NpgsqlConnection);
            await createParcelAddressTable.ExecuteNonQueryAsync();

            sql =
                @$"CREATE TABLE IF NOT EXISTS {SchemaLatestItems.Parcel} (
                    parcel_id uuid,
                    capakey varchar,
                    status text,
                    is_removed boolean
                );";
            await using var createParcelTable = new NpgsqlCommand(sql, NpgsqlConnection);
            await createParcelTable.ExecuteNonQueryAsync();

            sql =
                @$"CREATE TABLE IF NOT EXISTS {SchemaLatestItems.RoadSegment} (
                    id integer,
                    maintainer_id integer,
                    left_side_street_name_id integer,
                    right_side_street_name_id integer,
                    method_id integer,
                    status_id integer,
                    is_removed boolean,
                    version_timestamp TIMESTAMP WITH TIME ZONE
                );";
            await using var createRoadSegmentTable = new NpgsqlCommand(sql, NpgsqlConnection);
            await createRoadSegmentTable.ExecuteNonQueryAsync();
        }

        public async Task DisposeAsync()
        {
            await NpgsqlConnection.DisposeAsync();
        }
    }

    public class AccessToken
    {
        private readonly DateTime _expiresAt;

        public string Token { get; }

        // Let's regard it as expired 10 seconds before it actually expires.
        public bool IsExpired => _expiresAt < DateTime.Now.Add(TimeSpan.FromSeconds(10));

        public AccessToken(string token, int expiresIn)
        {
            _expiresAt = DateTime.Now.AddSeconds(expiresIn);
            Token = token;
        }
    }
}
