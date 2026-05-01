using DevEval.Application.Sales.Events;
using DevEval.Application.Sales.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DevEval.IoC.Kafka
{
    public class KafkaSaleCreatedEventProducer
        : BaseProducer<SaleCreatedEvent>, ISaleCreatedEventProducer
    {
        public KafkaSaleCreatedEventProducer(
            IConfiguration configuration,
            ILogger<KafkaSaleCreatedEventProducer> logger)
            : base(
                configuration,
                logger,
                "Kafka:Topics:SaleCreated",
                "sales.created",
                nameof(KafkaSaleCreatedEventProducer))
        {
        }

        public async Task PublishAsync(SaleCreatedEvent saleCreatedEvent, CancellationToken cancellationToken = default)
        {
            await base.PublishMessageAsync(saleCreatedEvent, cancellationToken);
        }
    }
}
