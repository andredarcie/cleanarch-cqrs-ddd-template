using DevEval.Application.Common.Errors;
using DevEval.Application.Sales.Commands;
using DevEval.Application.Sales.Services;
using DevEval.Domain.Entities.Sale;
using DevEval.Domain.Repositories;
using NSubstitute;

namespace DevEval.Test.Application.Sales.Handlers
{
    public class CancelSaleItemHandlerTests
    {
        private readonly ISaleRepository _saleRepositoryMock;
        private readonly ISaleEventPublisher _eventPublisherMock;
        private readonly CancelSaleItemHandler _handler;

        public CancelSaleItemHandlerTests()
        {
            _saleRepositoryMock = Substitute.For<ISaleRepository>();
            _eventPublisherMock = Substitute.For<ISaleEventPublisher>();
            _handler = new CancelSaleItemHandler(_saleRepositoryMock, _eventPublisherMock);
        }

        [Fact]
        public async Task Handle_ShouldCancelSaleItem_WhenSaleAndItemExist()
        {
            // Arrange
            var saleId = Guid.NewGuid();
            var itemId = Guid.NewGuid();
            var reason = "The product is no longer needed.";

            var saleItem = new SaleItem(1, 2, 10.0m, 0.0m);
            typeof(SaleItem).GetProperty(nameof(SaleItem.Id))?.SetValue(saleItem, itemId);

            var sale = new Sale("12345", Guid.NewGuid(), "Test Customer", Guid.NewGuid(), "Test Branch");
            typeof(Sale).GetProperty(nameof(Sale.Id))?.SetValue(sale, saleId);
            sale.AddItem(saleItem.ProductId, saleItem.Quantity, saleItem.UnitPrice);

            var addedItem = sale.Items.First();
            typeof(SaleItem).GetProperty(nameof(SaleItem.Id))?.SetValue(addedItem, itemId);

            _saleRepositoryMock.GetByIdAsync(saleId).Returns(sale);

            var command = new CancelSaleItemCommand(saleId, itemId, reason);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(addedItem.IsCancelled);
            await _saleRepositoryMock.Received(1).UpdateAsync(sale);
            await _eventPublisherMock.Received(1).PublishItemCancelledAsync(saleId, addedItem, reason);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFoundError_WhenSaleDoesNotExist()
        {
            // Arrange
            var saleId = Guid.NewGuid();
            var itemId = Guid.NewGuid();

            _saleRepositoryMock.GetByIdAsync(saleId).Returns((Sale?)null);

            var command = new CancelSaleItemCommand(saleId, itemId, "Not found");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);
            Assert.IsType<NotFoundError>(result.Errors.First());
            Assert.Equal($"Sale with ID {saleId} not found.", result.Errors.First().Message);
            await _saleRepositoryMock.Received(1).GetByIdAsync(saleId);
            await _saleRepositoryMock.DidNotReceive().UpdateAsync(Arg.Any<Sale>());
            await _eventPublisherMock.DidNotReceive().PublishItemCancelledAsync(Arg.Any<Guid>(), Arg.Any<SaleItem>(), Arg.Any<string>());
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFoundError_WhenItemDoesNotExistInSale()
        {
            // Arrange
            var saleId = Guid.NewGuid();
            var invalidItemId = Guid.NewGuid();

            var sale = new Sale("12345", Guid.NewGuid(), "Test Customer", Guid.NewGuid(), "Test Branch");
            typeof(Sale).GetProperty(nameof(Sale.Id))?.SetValue(sale, saleId);

            _saleRepositoryMock.GetByIdAsync(saleId).Returns(sale);

            var command = new CancelSaleItemCommand(saleId, invalidItemId, "Item not found");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);
            Assert.IsType<NotFoundError>(result.Errors.First());
            Assert.Equal($"Item with ID {invalidItemId} not found in sale {saleId}.", result.Errors.First().Message);
            await _saleRepositoryMock.Received(1).GetByIdAsync(saleId);
            await _saleRepositoryMock.DidNotReceive().UpdateAsync(Arg.Any<Sale>());
            await _eventPublisherMock.DidNotReceive().PublishItemCancelledAsync(Arg.Any<Guid>(), Arg.Any<SaleItem>(), Arg.Any<string>());
        }
    }
}
