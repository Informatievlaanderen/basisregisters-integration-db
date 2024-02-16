namespace Basisregisters.IntegrationDb.NationalRegistry.Repositories
{
    using System.Collections.Generic;
    using Dapper;
    using Npgsql;

    public interface IPostalCodeRepository
    {
        IEnumerable<string> GetAllPostalCodes();
    }

    public sealed class PostalCodeRepository : IPostalCodeRepository
    {
        private readonly string _connectionString;

        public PostalCodeRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IEnumerable<string> GetAllPostalCodes()
        {
            const string sql = @"select postal_code
                           from integration_postal.postal_latest_items
                           order by postal_code";

            using var connection = new NpgsqlConnection(_connectionString);

            var postalCodes = connection.Query<string>(sql);

            return postalCodes;
        }
    }
}
