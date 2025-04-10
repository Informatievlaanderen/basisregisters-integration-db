namespace Basisregisters.Integration.Veka
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon.DynamoDBv2;
    using Amazon.DynamoDBv2.DocumentModel;
    using Amazon.DynamoDBv2.Model;
    using Be.Vlaanderen.Basisregisters.Aws.DistributedMutex;
    using Microsoft.Extensions.Options;

    public interface IProjectionState
    {
        Task<int> GetLastPosition(CancellationToken cancellationToken);
        Task SetLastPosition(int lastPosition, CancellationToken ct);
    }

    public class DynamoDbProjectionState : IProjectionState
    {
        private const string PositionKeyColumnName = "resourceId";
        private const string PositionValueColumnName = "holderId";
        private const string LastUpdatedColumnName = "leaseExpiry";

        private readonly DistributedLockOptions _distributedLockOptions;
        private readonly IAmazonDynamoDB _dynamoDb;

        public DynamoDbProjectionState(
            IOptions<DistributedLockOptions> distributedLockOptions,
            IAmazonDynamoDB dynamoDb)
        {
            _distributedLockOptions = distributedLockOptions.Value;
            _dynamoDb = dynamoDb;
        }

        public async Task<int> GetLastPosition(CancellationToken cancellationToken)
        {
            var query = new GetItemRequest
            {
                TableName = _distributedLockOptions.TableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    { PositionKeyColumnName, new AttributeValue { S = "position" } }
                },
                ProjectionExpression = PositionValueColumnName,
                ConsistentRead = false
            };

            var response = await _dynamoDb.GetItemAsync(query, cancellationToken);

            return response is null
                ? 0
                : Convert.ToInt32(response.Item?.GetValueOrDefault(PositionValueColumnName)?.S);
        }

        public async Task SetLastPosition(int lastPosition, CancellationToken ct)
        {
            var table = Table.LoadTable(_dynamoDb, _distributedLockOptions.TableName);
            var now = DateTime.UtcNow;

            var writer = table.CreateBatchWrite();

            var doc = new Document
            {
                [PositionKeyColumnName] = "position",
                [PositionValueColumnName] = lastPosition.ToString(),
                [LastUpdatedColumnName] = now.ToString(CultureInfo.InvariantCulture)
            };

            writer.AddDocumentToPut(doc);

            await writer.ExecuteAsync(ct);
        }
    }

    public class FakeProjectionState : IProjectionState
    {
        public Task<int> GetLastPosition(CancellationToken cancellationToken)
        {
            const int meldingAfgerondEventPosition = 1618116; // melding b67520d6-a0c8-41ca-8cbd-f33d7e5f3321

            return Task.FromResult(meldingAfgerondEventPosition - 10);
        }

        public Task SetLastPosition(int lastPosition, CancellationToken ct)
        {
            return Task.CompletedTask;
        }
    }
}
