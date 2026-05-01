using DevEval.Application.Sales.Events;
using DevEval.Domain.Entities.Sale;
using Rebus.Bus;

namespace DevEval.Application.Sales.Services
{
    public class SaleEventPublisher : ISaleEventPublisher
    {
        private readonly IBus _bus;
        private readonly ISaleCreatedEventProducer _saleCreatedEventProducer;

        public SaleEventPublisher(IBus bus, ISaleCreatedEventProducer saleCreatedEventProducer)
        {
            _bus = bus;
            _saleCreatedEventProducer = saleCreatedEventProducer;
        }

        public async Task PublishSaleCreatedAsync(Sale sale)
        {
            var saleCreatedEvent = CreateSaleCreatedEvent(sale);

            await _bus.Publish(saleCreatedEvent);
            await _saleCreatedEventProducer.PublishAsync(saleCreatedEvent);
        }

        public async Task PublishSaleModifiedAsync(Sale sale)
        {
            var saleModifiedEvent = new SaleModifiedEvent
            {
                SaleId = sale.Id,
                SaleDate = sale.SaleDate,
                TotalAmount = sale.TotalAmount
            };

            await _bus.Publish(saleModifiedEvent);
        }

        public async Task PublishSaleCancelledAsync(Sale sale, string reason)
        {
            var saleCancelledEvent = new SaleCancelledEvent
            {
                SaleId = sale.Id,
                Reason = reason
            };

            await _bus.Publish(saleCancelledEvent);
        }

        public async Task PublishItemCancelledAsync(Guid saleId, SaleItem saleItem, string reason)
        {
            var itemCancelledEvent = new ItemCancelledEvent
            {
                ItemId = saleItem.ProductId,
                SaleId = saleId,
                Reason = reason
            };

            await _bus.Publish(itemCancelledEvent);
        }

        private static SaleCreatedEvent CreateSaleCreatedEvent(Sale sale)
        {
            return new SaleCreatedEvent
            {
                SaleId = sale.Id,
                SaleNumber = sale.SaleNumber,
                SaleDate = sale.SaleDate,
                CustomerId = sale.CustomerId,
                CustomerName = sale.CustomerName,
                BranchId = sale.BranchId,
                BranchName = sale.BranchName,
                TotalAmount = sale.TotalAmount,
                IsCancelled = sale.IsCancelled,
                ItemsCount = sale.Items.Count
            };
        }
    }
}
