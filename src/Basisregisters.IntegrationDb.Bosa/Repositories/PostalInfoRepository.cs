namespace Basisregisters.IntegrationDb.Bosa.Repositories
{
    using System.Collections.Generic;
    using Basisregisters.IntegrationDb.Bosa.Model;
    using Dapper;
    using IntegrationDB.Bosa.Infrastructure;
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
            const string sql = @$"
select postal.namespace, postal.postal_code as PostalCode, postal.version_timestamp as VersionTimestamp, name.name as DutchName, versions.version_timestamp
from integration_postal.postal_latest_items postal
join integration_postal.postal_information_name name on postal.postal_code = name.postal_code and name.language = 0
left join {DatabaseSetup.Schema}.{DatabaseSetup.PostalCrabVersionsTable} versions on postal.postal_code = versions.version_timestamp
order by postal.postal_code";

            using var connection = new NpgsqlConnection(_connectionString);

            var postalCodes = connection.Query<PostalInfo>(sql);

            return postalCodes;
        }
    }
}
