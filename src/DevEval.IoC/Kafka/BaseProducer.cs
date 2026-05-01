using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DevEval.IoC.Kafka
{
    public abstract class BaseProducer<TMessage> : IDisposable where TMessage : class
    {
        private readonly ILogger _logger;
        private readonly string _topic;
        private readonly string _producerName;
        private readonly IProducer<Null, string>? _producer;

        protected BaseProducer(
            IConfiguration configuration,
            ILogger logger,
            string topicConfigurationKey,
            string defaultTopicName,
            string producerName)
        {
            _logger = logger;
            _producerName = producerName;
            _topic = configuration[topicConfigurationKey] ?? defaultTopicName;

            var bootstrapServers = configuration["Kafka:BootstrapServers"];

            if (string.IsNullOrWhiteSpace(bootstrapServers))
            {
                _logger.LogInformation(
                    "Kafka bootstrap servers not configured. {ProducerName} will not publish to Kafka.",
                    _producerName);
                return;
            }

            var producerConfig = new ProducerConfig
            {
                BootstrapServers = bootstrapServers,
                ClientId = configuration["Kafka:ClientId"] ?? "deveval-webapi"
            };

            _producer = new ProducerBuilder<Null, string>(producerConfig).Build();
        }

        protected async Task PublishMessageAsync(TMessage message, CancellationToken cancellationToken = default)
        {
            if (_producer == null)
            {
                return;
            }

            var payload = JsonSerializer.Serialize(message);

            try
            {
                await _producer.ProduceAsync(
                    _topic,
                    new Message<Null, string> { Value = payload },
                    cancellationToken);
            }
            catch (ProduceException<Null, string> ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to publish {ProducerName} message to Kafka topic {Topic}.",
                    _producerName,
                    _topic);
                throw;
            }
        }

        public void Dispose()
        {
            _producer?.Flush(TimeSpan.FromSeconds(5));
            _producer?.Dispose();
        }
    }
}
