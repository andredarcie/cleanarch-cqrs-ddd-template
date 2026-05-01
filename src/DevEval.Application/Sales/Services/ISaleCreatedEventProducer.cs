using DevEval.Application.Sales.Events;

namespace DevEval.Application.Sales.Services
{
    public interface ISaleCreatedEventProducer
    {
        Task PublishAsync(SaleCreatedEvent saleCreatedEvent, CancellationToken cancellationToken = default);
    }
}
