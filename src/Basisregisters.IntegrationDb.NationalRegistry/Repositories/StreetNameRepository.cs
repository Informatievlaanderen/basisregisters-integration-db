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
where nis_code = @NisCode";

            using var connection = new NpgsqlConnection(_connectionString);

            var streetNames = connection.Query<StreetName>(sql, new { NisCode = nisCode });

            return streetNames;
        }
    }

    public record StreetName(
        int StreetNamePersistentLocalId,
        string? NameDutch,
        string? NameFrench,
        string? NameGerman,
        string? NameEnglish,
        string? HomonymAdditionDutch,
        string? HomonymAdditionFrench,
        string? HomonymAdditionGerman,
        string? HomonymAdditionEnglish);
}
