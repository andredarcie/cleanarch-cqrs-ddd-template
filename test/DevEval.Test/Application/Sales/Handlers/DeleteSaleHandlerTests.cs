using DevEval.Application.Common.Errors;
using DevEval.Application.Sales.Commands;
using DevEval.Application.Sales.Handlers;
using DevEval.Application.Sales.Services;
using DevEval.Domain.Entities.Sale;
using DevEval.Domain.Repositories;
using NSubstitute;

namespace DevEval.Test.Application.Sales.Handlers
{
    public class DeleteSaleHandlerTests
    {
        private readonly ISaleRepository _saleRepositoryMock;
        private readonly ISaleEventPublisher _eventPublisherMock;
        private readonly DeleteSaleHandler _handler;

        public DeleteSaleHandlerTests()
        {
            _saleRepositoryMock = Substitute.For<ISaleRepository>();
            _eventPublisherMock = Substitute.For<ISaleEventPublisher>();
            _handler = new DeleteSaleHandler(_saleRepositoryMock, _eventPublisherMock);
        }

        [Fact]
        public async Task Handle_ShouldCancelSale_WhenSaleExists()
        {
            // Arrange
            var saleId = Guid.NewGuid();
            var sale = new Sale("SALE-001", Guid.NewGuid(), "John Doe", Guid.NewGuid(), "Main Branch");
            typeof(Sale).GetProperty(nameof(Sale.Id))?.SetValue(sale, saleId);
            sale.AddItem(1, 4, 10m);

            _saleRepositoryMock.GetByIdAsync(saleId).Returns(sale);
            _saleRepositoryMock.UpdateAsync(sale).Returns(sale);

            // Act
            var result = await _handler.Handle(new DeleteSaleCommand(saleId), CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(sale.IsCancelled);
            Assert.All(sale.Items, item => Assert.True(item.IsCancelled));
            await _saleRepositoryMock.Received(1).UpdateAsync(sale);
            await _saleRepositoryMock.DidNotReceive().DeleteAsync(Arg.Any<Guid>());
            await _eventPublisherMock.Received(1).PublishSaleCancelledAsync(sale, "Cancelled by user");
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenSaleDoesNotExist()
        {
            // Arrange
            var saleId = Guid.NewGuid();
            _saleRepositoryMock.GetByIdAsync(saleId).Returns((Sale?)null);

            // Act
            var result = await _handler.Handle(new DeleteSaleCommand(saleId), CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);
            Assert.IsType<NotFoundError>(result.Errors.First());
            await _saleRepositoryMock.DidNotReceive().UpdateAsync(Arg.Any<Sale>());
            await _eventPublisherMock.DidNotReceive().PublishSaleCancelledAsync(Arg.Any<Sale>(), Arg.Any<string>());
        }
    }
}
