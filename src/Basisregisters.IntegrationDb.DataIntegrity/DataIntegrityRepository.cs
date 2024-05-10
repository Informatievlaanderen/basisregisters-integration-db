namespace Basisregisters.IntegrationDb.DataIntegrity
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Dapper;
    using Npgsql;
    using SuspiciousCases.Views.Internal;
    using SuspiciousCases.Infrastructure;

    public sealed class DataIntegrityRepository(string connectionString)
    {
        public async Task<IList<DataIntegrityError>> GetErrors()
        {
            const string sql = $"""
                 SELECT
                     '{nameof(ActiveAddressButInActiveStreetName)}' as "Message",
                     count(*) as "Count"
                 FROM {Schema.SuspiciousCases}.{ActiveAddressButInActiveStreetName.ViewName}
                 UNION
                 SELECT
                     '{nameof(ActiveBuildingUnitOutsideBuilding)}' as "Message",
                     count(*) as "Count"
                 FROM {Schema.SuspiciousCases}.{ActiveBuildingUnitOutsideBuilding.ViewName}
                 UNION
                 SELECT
                     '{nameof(BuildingWithMoreThanOneUnitWithoutCommonUnit)}' as "Message",
                     count(*) as "Count"
                 FROM {Schema.SuspiciousCases}.{BuildingWithMoreThanOneUnitWithoutCommonUnit.ViewName}
                 UNION
                 SELECT
                     '{nameof(BuildingWithOneOrNoUnitWithCommonUnit)}' as "Message",
                     count(*) as "Count"
                 FROM {Schema.SuspiciousCases}.{BuildingWithOneOrNoUnitWithCommonUnit.ViewName}
                 UNION
                 SELECT
                     '{nameof(InactiveAddressLinkedToParcelOrBuildingUnit)}' as "Message",
                     count(*) as "Count"
                 FROM {Schema.SuspiciousCases}.{InactiveAddressLinkedToParcelOrBuildingUnit.ViewName}
                 UNION
                 SELECT
                     '{nameof(InactiveBuildingUnitLinkedToAddress)}' as "Message",
                     count(*) as "Count"
                 FROM {Schema.SuspiciousCases}.{InactiveBuildingUnitLinkedToAddress.ViewName}
                 UNION
                 SELECT
                     '{nameof(InactiveParcelLinkedToAddress)}' as "Message",
                     count(*) as "Count"
                 FROM {Schema.SuspiciousCases}.{InactiveParcelLinkedToAddress.ViewName};
                 """;

            await using var connection = new NpgsqlConnection(connectionString);

            var result = (await connection.QueryAsync<DataIntegrityError>(sql, commandTimeout: 5 * 60))
                .ToList()
                .Where(x => x.Count > 0)
                .ToList();

            return result;
        }
    }

    public class DataIntegrityError
    {
        public string Message { get; set; }
        public long Count { get; set; }

        public DataIntegrityError(
            string message,
            long count)
        {
            Message = message;
            Count = count;
        }
    }
}
