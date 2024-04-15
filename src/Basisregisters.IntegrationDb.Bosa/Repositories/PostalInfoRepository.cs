namespace Basisregisters.IntegrationDb.Bosa.Repositories
{
    using System.Collections.Generic;
    using Basisregisters.IntegrationDb.Bosa.Model;
    using Dapper;
    using Npgsql;

    public interface IPostalInfoRepository
    {
        IEnumerable<PostalInfo> GetAllPostalInfos();
    }

    public sealed class PostalInfoRepository : IPostalInfoRepository
    {
        private readonly string _connectionString;

        public PostalInfoRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IEnumerable<PostalInfo> GetAllPostalInfos()
        {
            const string sql = @"
select postal.namespace, postal.postal_code as PostalCode, postal.version_timestamp as VersionTimestamp, name.name as DutchName
from integration_postal.postal_latest_items postal
join integration_postal.postal_information_name name on postal.postal_code = name.postal_code and name.language = 0
order by postal.postal_code";

            using var connection = new NpgsqlConnection(_connectionString);

            var postalCodes = connection.Query<PostalInfo>(sql);

            return postalCodes;
        }
    }
}
