namespace Basisregisters.IntegrationDb.NationalRegistry.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Dapper;
    using NetTopologySuite.Geometries;
    using Npgsql;

    public interface IMunicipalityRepository
    {
        IList<Municipality> GetMunicipalities();
    }

    public class MunicipalityRepository : IMunicipalityRepository
    {
        private readonly string _connectionString;
        private IList<Municipality>? _municipalities;

        public MunicipalityRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IList<Municipality> GetMunicipalities()
        {
            if (_municipalities is not null)
            {
                return _municipalities;
            }

            const string sql = @"SELECT
                g.nis_code as NisCode,
                ST_PointOnSurface(g.geometry) as Centroid
            FROM integration_municipality.municipality_latest_items m
            JOIN integration_municipality.municipality_geometries g on m.nis_code = g.nis_code
            WHERE m.is_removed = false";

            // https://www.npgsql.org/doc/release-notes/7.0.html#managing-type-mappings-at-the-connection-level-is-no-longer-supported
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(_connectionString);
            dataSourceBuilder.UseNetTopologySuite();
            using var dataSource = dataSourceBuilder.Build();
            using var connection = dataSource.CreateConnection();

            _municipalities = connection.Query<Municipality>(sql).ToList();

            return _municipalities;
        }
    }

    public class Municipality
    {
        public Municipality(
            string nisCode,
            Geometry centroid)
        {
            NisCode = nisCode;
            Centroid = (Point)centroid;
        }

        public string NisCode { get; }
        public Point Centroid { get; }
    }
}
