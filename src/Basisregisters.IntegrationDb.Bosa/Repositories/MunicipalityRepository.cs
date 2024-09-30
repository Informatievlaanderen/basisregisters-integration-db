namespace Basisregisters.IntegrationDb.Bosa.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Be.Vlaanderen.Basisregisters.GrAr.Common;
    using Dapper;
    using IntegrationDB.Bosa.Infrastructure;
    using Model.Database;
    using Npgsql;

    public interface IMunicipalityRepository
    {
        IEnumerable<Municipality> GetFlemish();
    }

    public class MunicipalityRepository : IMunicipalityRepository
    {
        private readonly string _connectionString;

        public MunicipalityRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IEnumerable<Municipality> GetFlemish()
        {
            const string sql = $"""

                                select
                                    municipality.namespace
                                    , municipality.nis_code as NisCode
                                    , municipality.version_timestamp as VersionTimestamp
                                    , municipality.name_dutch as DutchName
                                    , municipality.name_french as FrenchName
                                    , municipality.name_german as GermanName
                                    , municipality.name_english as EnglishName
                                    , version.version_timestamp as CrabVersionTimestamp
                                    , municipality.status as Status
                                from integration_municipality.municipality_latest_items municipality
                                left join {DatabaseSetup.Schema}.{DatabaseSetup.MunicipalityCrabVersionsTable} version on municipality.nis_code = version.nis_code
                                where municipality.is_flemish_region = true
                                order by municipality.nis_code
                                """;

            using var connection = new NpgsqlConnection(_connectionString);

            var municipalities = connection.Query<Municipality>(sql);

            return municipalities.Where(x => RegionFilter.IsFlemishRegion(x.NisCode));
        }
    }
}
