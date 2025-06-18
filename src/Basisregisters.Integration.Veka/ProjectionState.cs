namespace Basisregisters.Integration.Veka
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon.DynamoDBv2;
    using Amazon.DynamoDBv2.DocumentModel;
    using Amazon.DynamoDBv2.Model;
    using Be.Vlaanderen.Basisregisters.Aws.DistributedMutex;
    using Microsoft.Extensions.Configuration;

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

        private readonly string _projectionStateTableName;
        private readonly IAmazonDynamoDB _dynamoDb;

        public DynamoDbProjectionState(
            IConfiguration configuration,
            IAmazonDynamoDB dynamoDb)
        {
            _projectionStateTableName = configuration.GetValue<string>("ProjectionStateTableName")
                                        ?? throw new ArgumentNullException(nameof(configuration), "ProjectionStateTableName is not configured.");
            _dynamoDb = dynamoDb;
        }

        public async Task<int> GetLastPosition(CancellationToken cancellationToken)
        {
            await EnsureTableExists();

            var query = new GetItemRequest
            {
                TableName = _projectionStateTableName,
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
            var table = Table.LoadTable(_dynamoDb, _projectionStateTableName);
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

        private async Task EnsureTableExists()
        {
            await new DynamoDBMutex(_dynamoDb,
                    new DynamoDBMutexSettings
                    {
                        TableName = _projectionStateTableName,
                        CreateTableIfNotExists = true
                    })
                .CreateTableAsync();
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

    // Lighter version of https://github.com/Informatievlaanderen/aws-distributed-mutex/blob/main/src/Be.Vlaanderen.Basisregisters.Aws.DistributedMutex/DynamoDBMutex.cs
    // ReSharper disable once InconsistentNaming
    public class DynamoDBMutex
    {
        private readonly DynamoDBMutexSettings _settings;
        private readonly IAmazonDynamoDB _client;

        private static class ColumnNames
        {
            public const string ResourceId = "resourceId";
        }

        public DynamoDBMutex(
            IAmazonDynamoDB client,
            DynamoDBMutexSettings settings)
        {
            _settings = settings;
            _client = client;
        }

        public async Task CreateTableAsync()
        {
            try
            {
                if (await DoesTableExists())
                {
                    return;
                }

                await _client.CreateTableAsync(new CreateTableRequest
                {
                    TableName = _settings.TableName,
                    AttributeDefinitions =
                    [
                        new AttributeDefinition
                        {
                            AttributeType = ScalarAttributeType.S,
                            AttributeName = ColumnNames.ResourceId
                        }
                    ],
                    KeySchema =
                    [
                        new KeySchemaElement
                        {
                            KeyType = KeyType.HASH,
                            AttributeName = ColumnNames.ResourceId
                        }
                    ],
                    BillingMode = BillingMode.PAY_PER_REQUEST
                });

                // need to wait a bit since the table has just been created
                await Task.Delay(TimeSpan.FromSeconds(10));
            }
            catch (ResourceInUseException)
            {
                // ignore, already exists
            }
        }

        private async Task<bool> DoesTableExists()
        {
            // https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/LowLevelDotNetWorkingWithTables.html#LowLeveldotNetListTables
            // Initial value for the first page of table names.
            string? lastEvaluatedTableName = null;
            do
            {
                // Create a request object to specify optional parameters.
                var request = new ListTablesRequest { ExclusiveStartTableName = lastEvaluatedTableName };

                var result = await _client.ListTablesAsync(request);
                if (result.TableNames.Any(t => t == _settings.TableName))
                {
                    return true;
                }

                lastEvaluatedTableName = result.LastEvaluatedTableName;
            } while (lastEvaluatedTableName != null);

            return false;
        }
    }
}
