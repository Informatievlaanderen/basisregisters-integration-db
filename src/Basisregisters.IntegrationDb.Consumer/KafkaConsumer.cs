namespace Basisregisters.IntegrationDb.Consumer
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.MessageHandling.Kafka.Consumer;
    using Confluent.Kafka;
    using Newtonsoft.Json;

    public interface IKafkaConsumer
    {
        Task Consume(Func<dynamic?, string, Task> handle, CancellationToken stoppingToken);
    }

    public class KafkaConsumer : IKafkaConsumer
    {
        private readonly ConsumerOptions _consumerOptions;
        private readonly IConsumer<string, string> _consumer;

        public KafkaConsumer(ConsumerOptions options)
        {
            _consumerOptions = options;
            var config = new ConsumerConfig
            {
                BootstrapServers = options.BootstrapServers,
                GroupId = options.ConsumerGroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = true,
                //EnableAutoOffsetStore = false
            };

            if (options.SaslAuthentication.HasValue)
            {
                config.SecurityProtocol = SecurityProtocol.SaslSsl;
                config.SaslMechanism = SaslMechanism.Plain;
                config.SaslUsername = options.SaslAuthentication.Value.Username;
                config.SaslPassword = options.SaslAuthentication.Value.Password;
            }
            var consumerBuilder = new ConsumerBuilder<string, string>(config).SetValueDeserializer(Deserializers.Utf8);
            if (options.Offset.HasValue)
            {
                consumerBuilder.SetPartitionsAssignedHandler((_, topicPartitions) =>
                {
                    var partitionOffset = topicPartitions.Select(x => new TopicPartitionOffset(
                        x.Topic,
                        x.Partition,
                        new Offset(options.Offset.Value)));
                    return partitionOffset;
                });
            }

            _consumer = consumerBuilder.Build();
        }

        public async Task Consume(Func<dynamic?, string, Task> handle, CancellationToken stoppingToken)
        {
            try
            {
                _consumer.Subscribe(_consumerOptions.Topic);

                while (!stoppingToken.IsCancellationRequested)
                {
                    var consumeResult = _consumer.Consume(stoppingToken);

                    if (consumeResult == null) //if no message is found, returns null
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(300), stoppingToken);
                        continue;
                    }

                    Console.WriteLine($"Consumer received message offset {consumeResult.Offset.Value}");

                    var msg = JsonConvert.DeserializeObject<dynamic>(consumeResult.Message.Value);

                    var puri = consumeResult.Message.Key;
                    var objectId = new Uri(puri).Segments.Last();

                    await handle(msg, objectId);

                    //_consumer.StoreOffset(consumeResult);
                    //_consumer.Commit(consumeResult);
                }
            }
            finally
            {
                _consumer.Unsubscribe();
            }
        }
    }
}
