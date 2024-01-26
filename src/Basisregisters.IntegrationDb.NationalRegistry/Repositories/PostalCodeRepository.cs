namespace Basisregisters.IntegrationDb.NationalRegistry.Repositories
{
    using System.Collections.Generic;

    public interface IPostalCodeRepository
    {
        IEnumerable<string> GetAllPostalCodes();
    }

    public sealed class PostalCodeRepository : IPostalCodeRepository
    {
        private readonly string _connectionString;

        public PostalCodeRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IEnumerable<string> GetAllPostalCodes()
        {
            using (new Npgsql.NpgsqlConnection(_connectionString))
            {
                //TODO: Implement integration db in postal repository
            }

            return new List<string>();
        }
    }
}
