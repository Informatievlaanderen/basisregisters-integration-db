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
    persistent_local_id as StreetNamePersistentLocalId,
    name_dutch as NameDutch,
    name_french as NameFrench,
    name_german as NameGerman,
    name_english as NameEnglish,
    homonym_addition_dutch as HomonymAdditionDutch,
    homonym_addition_french as HomonymAdditionFrench,
    homonym_addition_german as HomonymAdditionGerman,
    homonym_addition_english as HomonymAdditionEnglish
from integration_streetname.streetname_latest_items
where nis_code = @NisCode and status in (0,1) and is_removed = false";

            using var connection = new NpgsqlConnection(_connectionString);

            var streetNames = connection.Query<StreetName>(sql, new { NisCode = nisCode });

            return streetNames;
        }
    }

    public class StreetName
    {
        public StreetName(int streetNamePersistentLocalId, string? nameDutch, string? nameFrench, string? nameGerman, string? nameEnglish, string? homonymAdditionDutch, string? homonymAdditionFrench, string? homonymAdditionGerman, string? homonymAdditionEnglish)
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
    }
}
