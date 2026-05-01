using System.Collections.Concurrent;
using DevEval.Application.Sales.Events;
using DevEval.Application.Sales.Services;

namespace DevEval.IntegrationTests.Infrastructure;

public sealed class FakeSaleCreatedEventProducer : ISaleCreatedEventProducer
{
    private readonly ConcurrentQueue<SaleCreatedEvent> _published = new();

    public IReadOnlyList<SaleCreatedEvent> Published => _published.ToArray();

    public Task PublishAsync(SaleCreatedEvent saleCreatedEvent, CancellationToken cancellationToken = default)
    {
        _published.Enqueue(saleCreatedEvent);
        return Task.CompletedTask;
    }
}
