namespace Basisregisters.IntegrationDb.Bosa.Repositories
{
    using System.Collections.Generic;
    using Dapper;
    using IntegrationDB.Bosa.Infrastructure;
    using Model.Database;
    using Npgsql;

    public interface IMunicipalityRepository
    {
        IEnumerable<Municipality> GetAll();
    }

    public class MunicipalityRepository : IMunicipalityRepository
    {
        private readonly string _connectionString;

        public MunicipalityRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IEnumerable<Municipality> GetAll()
        {
            const string sql = @$"
select
    municipality.namespace
    , municipality.nis_code as NisCode
    , municipality.version_timestamp as VersionTimestamp
    , municipality.name_dutch as DutchName
    , version.version_timestamp as CrabVersionTimestamp
from integration_municipality.municipality_latest_items municipality
left join {DatabaseSetup.Schema}.{DatabaseSetup.MunicipalityCrabVersionsTable} version on municipality.nis_code = version.nis_code
where municipality.nis_code in (
    select distinct postal.nis_code
from integration_postal.postal_latest_items postal
join integration_postal.postal_information_name name on postal.postal_code = name.postal_code and name.language = 0
left join {DatabaseSetup.Schema}.{DatabaseSetup.PostalCrabVersionsTable} version on postal.postal_code = version.postal_code
where (postal.postal_code LIKE '8%' OR postal.postal_code LIKE '9%' OR postal.postal_code LIKE '3%' OR postal.postal_code LIKE '2%'
   OR postal.postal_code LIKE '15%' OR postal.postal_code LIKE '16%' OR postal.postal_code LIKE '17%' OR postal.postal_code LIKE '18%'
   OR postal.postal_code LIKE '19%') and postal.nis_code is not null
)
order by municipality.nis_code";

            using var connection = new NpgsqlConnection(_connectionString);

            var municipalities = connection.Query<Municipality>(sql);

            return municipalities;
        }
    }
}
