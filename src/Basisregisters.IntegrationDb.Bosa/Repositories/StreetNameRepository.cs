namespace Basisregisters.IntegrationDb.Bosa.Repositories
{
    using System.Collections.Generic;
    using Dapper;
    using Model.Database;
    using Npgsql;

    public interface IStreetNameRepository
    {
        IEnumerable<StreetName> GetFlemish();
    }

    public class StreetNameRepository : IStreetNameRepository
    {
        private readonly string _connectionString;

        public StreetNameRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IEnumerable<StreetName> GetFlemish()
        {
            const string sql = @"
select
	s.persistent_local_id as StreetNamePersistentLocalId
	, s.namespace as Namespace
	, s.status as Status
	, s.version_timestamp as VersionTimestamp
	, sc.version_timestamp as CrabVersionTimestamp
	, c.created_on as CreatedOn
    , s.name_dutch as NameDutch
	, s.name_french as NameFrench
	, s.name_german as NameGerman
	, m.nis_code as NisCode
	, m.namespace as MunicipalityNamespace
	, m.version_timestamp as MunicipalityVersionTimestamp
	, mc.version_timestamp as MunicipalityCrabVersionTimestamp
	, m.official_language_dutch as HasOfficialLanguageDutch
	, m.official_language_french as HasOfficialLanguageFrench
	, m.official_language_german as HasOfficialLanguageGerman
from integration_streetname.streetname_latest_items s
inner join integration_municipality.municipality_latest_items m on m.municipality_id = s.municipality_id and m.is_flemish_region = true
left join integration_bosa.municipality_crab_versions mc on mc.nis_code = m.nis_code
left join integration_bosa.streetname_crab_versions sc on sc.streetname_persistent_local_id = s.persistent_local_id
inner join
(select sv.persistent_local_id, min(sv.created_on_timestamp) as created_on
from integration_streetname.streetname_versions sv
group by sv.persistent_local_id) c on s.persistent_local_id = c.persistent_local_id
where s.is_removed = false
order by m.nis_code, s.persistent_local_id";

            using var connection = new NpgsqlConnection(_connectionString);

            var postalCodes = connection.Query<StreetName>(sql);

            return postalCodes;
        }
    }
}
