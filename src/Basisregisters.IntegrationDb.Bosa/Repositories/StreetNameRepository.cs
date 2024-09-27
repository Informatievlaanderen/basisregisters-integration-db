namespace Basisregisters.IntegrationDb.Bosa.Repositories
{
    using System.Collections.Generic;
    using Basisregisters.IntegrationDB.Bosa.Infrastructure;
    using Dapper;
    using Model.Database;
    using Npgsql;

    public interface IStreetNameRepository
    {
        IEnumerable<StreetName> GetFlemish();
        IEnumerable<StreetNameIdentifier> GetFlemishIdentifiers();
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
            var sql = $"""
                       select
                           s.namespace as Namespace
                       	, s.persistent_local_id as StreetNamePersistentLocalId
                       	, s.status as Status
                       	, s.version_timestamp as VersionTimestamp
                       	, sc.version_timestamp as CrabVersionTimestamp
                           , sc.created_on as CrabCreatedOn
                       	, c.created_on as CreatedOn
                           , s.name_dutch as NameDutch
                       	, s.name_french as NameFrench
                       	, s.name_german as NameGerman
                       	, m.nis_code as NisCode
                       	, m.namespace as MunicipalityNamespace
                       	, m.version_timestamp as MunicipalityVersionTimestamp
                       	, mc.version_timestamp as MunicipalityCrabVersionTimestamp
                       from integration_streetname.streetname_latest_items s
                       inner join integration_municipality.municipality_latest_items m
                           on m.municipality_id = s.municipality_id and m.is_flemish_region = true and m.status <> {(int)MunicipalityStatus.Retired}
                       left join {DatabaseSetup.Schema}.{DatabaseSetup.MunicipalityCrabVersionsTable} mc on mc.nis_code = m.nis_code
                       left join {DatabaseSetup.Schema}.{DatabaseSetup.StreetNameCrabVersionsTable} sc on sc.streetname_persistent_local_id = s.persistent_local_id
                       inner join
                       (select sv.persistent_local_id, min(sv.created_on_timestamp) as created_on
                       from integration_streetname.streetname_versions sv
                       group by sv.persistent_local_id) c on s.persistent_local_id = c.persistent_local_id
                       where s.is_removed = false
                       order by m.nis_code, s.persistent_local_id
                       """;

            using var connection = new NpgsqlConnection(_connectionString);

            var streetNames = connection.Query<StreetName>(sql);

            return streetNames;
        }

        public IEnumerable<StreetNameIdentifier> GetFlemishIdentifiers()
        {
            var sql = $"""
                       select
                       	s.namespace as Namespace
                       	, s.persistent_local_id as StreetNamePersistentLocalId
                           , s.version_timestamp as VersionTimestamp
                       	, sc.version_timestamp as CrabVersionTimestamp
                       	, m.nis_code as NisCode
                       from integration_streetname.streetname_latest_items s
                       inner join integration_municipality.municipality_latest_items m
                           on m.municipality_id = s.municipality_id and m.is_flemish_region = true and m.status <> {(int)MunicipalityStatus.Retired}
                       left join {DatabaseSetup.Schema}.{DatabaseSetup.StreetNameCrabVersionsTable} sc on sc.streetname_persistent_local_id = s.persistent_local_id
                       where s.is_removed = false
                       order by m.nis_code, s.persistent_local_id
                       """;

            using var connection = new NpgsqlConnection(_connectionString);

            var identifiers = connection.Query<StreetNameIdentifier>(sql);

            return identifiers;
        }
    }
}
