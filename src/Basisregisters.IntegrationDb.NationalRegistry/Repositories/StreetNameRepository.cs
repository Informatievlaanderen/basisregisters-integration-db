namespace Basisregisters.IntegrationDb.NationalRegistry.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Dapper;
    using NetTopologySuite.Geometries;
    using Npgsql;

    public interface IStreetNameRepository
    {
        IList<StreetName> GetStreetNames();
    }

    public class StreetNameRepository : IStreetNameRepository
    {
        private readonly string _connectionString;
        private IList<StreetName>? _streetNames;

        public StreetNameRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IList<StreetName> GetStreetNames()
        {
            if (_streetNames is not null)
            {
                return _streetNames;
            }

            const string sql = @"
        select
            q.StreetNamePersistentLocalId,
            q.NameDutch,
	        q.NameFrench,
	        q.NameGerman,
	        q.NameEnglish,
	        q.HomonymAdditionDutch,
	        q.HomonymAdditionFrench,
	        q.HomonymAdditionGerman,
	        q.HomonymAdditionEnglish,
	        q.NisCode,
	        q.MunicipalityName,
            (CASE WHEN q.geometry IS NOT NULL THEN ST_ClosestPoint(q.geometry, ST_Centroid(q.geometry)) ELSE NULL END) as Centroid
        from (
            select
            s.persistent_local_id as StreetNamePersistentLocalId,
            s.name_dutch as NameDutch,
	        s.name_french as NameFrench,
	        s.name_german as NameGerman,
	        s.name_english as NameEnglish,
	        s.homonym_addition_dutch as HomonymAdditionDutch,
	        s.homonym_addition_french as HomonymAdditionFrench,
	        s.homonym_addition_german as HomonymAdditionGerman,
	        s.homonym_addition_english as HomonymAdditionEnglish,
	        s.nis_code as NisCode,
	        m.name_dutch as MunicipalityName,
	        (
		        select ST_LineMerge(
		            ST_Collect(
				        ST_Transform(rs.geometry, ST_Srid(rs.geometry))
		            )
	            )
		        from integration_road.road_segment_latest_items rs
		        where rs.is_removed = false and (rs.left_side_street_name_id = s.persistent_local_id or rs.right_side_street_name_id = s.persistent_local_id)
	        ) as Geometry
            from integration_streetname.streetname_latest_items s
            LEFT JOIN integration_municipality.municipality_latest_items m on s.nis_code = m.nis_code
            where s.is_removed = false and s.oslo_status in ('Voorgesteld', 'InGebruik')
        ) q";

            // https://www.npgsql.org/doc/release-notes/7.0.html#managing-type-mappings-at-the-connection-level-is-no-longer-supported
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(_connectionString);
            dataSourceBuilder.UseNetTopologySuite();
            using var dataSource = dataSourceBuilder.Build();
            using var connection = dataSource.CreateConnection();

            _streetNames = connection.Query<StreetName>(sql).ToList();

            return _streetNames;
        }
    }

    public class StreetName
    {
        public StreetName(
            int streetNamePersistentLocalId,
            string? nameDutch,
            string? nameFrench,
            string? nameGerman,
            string? nameEnglish,
            string? homonymAdditionDutch,
            string? homonymAdditionFrench,
            string? homonymAdditionGerman,
            string? homonymAdditionEnglish,
            string nisCode,
            string municipalityName,
            Point? centroid)
        {
            StreetNamePersistentLocalId = streetNamePersistentLocalId;
            NameDutch = nameDutch;
            NameFrench = nameFrench;
            NameGerman = nameGerman;
            NameEnglish = nameEnglish;
            HomonymAdditionDutch = homonymAdditionDutch;
            HomonymAdditionFrench = homonymAdditionFrench;
            HomonymAdditionGerman = homonymAdditionGerman;
            HomonymAdditionEnglish = homonymAdditionEnglish;
            NisCode = nisCode;
            MunicipalityName = municipalityName;
            Centroid = centroid;
        }
        public int StreetNamePersistentLocalId { get; }
        public string? NameDutch { get; }
        public string? NameFrench { get; }
        public string? NameGerman { get; }
        public string? NameEnglish { get; }
        public string? HomonymAdditionDutch { get; }
        public string? HomonymAdditionFrench { get; }
        public string? HomonymAdditionGerman { get; }
        public string? HomonymAdditionEnglish { get; }
        public string NisCode { get; }
        public string MunicipalityName { get; }
        public Point? Centroid { get; }
    }
}
