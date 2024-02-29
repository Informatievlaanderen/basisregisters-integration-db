namespace Basisregisters.IntegrationDb.NationalRegistry.Repositories
{
    using System.Collections.Generic;
    using Dapper;
    using Npgsql;

    public interface IStreetNameRepository
    {
        IEnumerable<StreetName> GetStreetNamesByNisCode(string nisCode);
    }

    public class StreetNameRepository : IStreetNameRepository
    {
        private readonly string _connectionString;

        public StreetNameRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IEnumerable<StreetName> GetStreetNamesByNisCode(string nisCode)
        {
            const string sql = @"select
            s.persistent_local_id as StreetNamePersistentLocalId,
            s.name_dutch as NameDutch,
	        s.name_french as NameFrench,
	        s.name_german as NameGerman,
	        s.name_english as NameEnglish,
	        s.homonym_addition_dutch as HomonymAdditionDutch,
	        s.homonym_addition_french as HomonymAdditionFrench,
	        s.homonym_addition_german as HomonymAdditionGerman,
	        s.homonym_addition_english as HomonymAdditionEnglish,
	        m.name_dutch as MunicipalityName
            from integration_streetname.streetname_latest_items s
            LEFT JOIN integration_municipality.municipality_latest_items m on s.nis_code = m.nis_code
            where s.nis_code = @NisCode and s.status in (0,1) and s.is_removed = false";

            using var connection = new NpgsqlConnection(_connectionString);

            var streetNames = connection.Query<StreetName>(sql, new { NisCode = nisCode });

            return streetNames;
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
            string municipalityName)
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
            MunicipalityName = municipalityName;
        }
        public int StreetNamePersistentLocalId { get; set; }
        public string? NameDutch { get; set; }
        public string? NameFrench { get; set; }
        public string? NameGerman { get; set; }
        public string? NameEnglish { get; set; }
        public string? HomonymAdditionDutch { get; set; }
        public string? HomonymAdditionFrench { get; set; }
        public string? HomonymAdditionGerman { get; set; }
        public string? HomonymAdditionEnglish { get; set; }
        public string MunicipalityName { get; set; }
    }
}
