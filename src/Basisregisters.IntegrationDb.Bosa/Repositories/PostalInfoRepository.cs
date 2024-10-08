namespace Basisregisters.IntegrationDb.Bosa.Repositories
{
    using System.Collections.Generic;
    using Dapper;
    using IntegrationDB.Bosa.Infrastructure;
    using Model.Database;
    using Npgsql;

    public interface IPostalInfoRepository
    {
        IEnumerable<PostalInfo> GetFlemish();
    }

    public sealed class PostalInfoRepository : IPostalInfoRepository
    {
        private readonly string _connectionString;

        public PostalInfoRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IEnumerable<PostalInfo> GetFlemish()
        {
            const string sql = $"""

                                select
                                    postal.namespace
                                    , postal.postal_code as PostalCode
                                    , postal.version_timestamp as VersionTimestamp
                                    , name.name as DutchName
                                from integration_postal.postal_latest_items postal
                                join integration_postal.postal_information_name name on postal.postal_code = name.postal_code
                                where postal.postal_code LIKE '8%' OR postal.postal_code LIKE '9%' OR postal.postal_code LIKE '3%' OR postal.postal_code LIKE '2%'
                                   OR postal.postal_code LIKE '15%' OR postal.postal_code LIKE '16%' OR postal.postal_code LIKE '17%' OR postal.postal_code LIKE '18%'
                                   OR postal.postal_code LIKE '19%'
                                order by postal.postal_code
                                """;

            using var connection = new NpgsqlConnection(_connectionString);

            var postalCodes = connection.Query<PostalInfo>(sql);

            return postalCodes;
        }
    }
}
