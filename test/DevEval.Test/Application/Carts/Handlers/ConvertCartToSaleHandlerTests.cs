using DevEval.Application.Carts.Commands;
using DevEval.Application.Carts.Handlers;
using DevEval.Application.Common.Errors;
using DevEval.Application.Sales.Services;
using DevEval.Domain.Entities.Cart;
using DevEval.Domain.Entities.Sale;
using DevEval.Domain.Entities.User;
using DevEval.Domain.Enums;
using DevEval.Domain.Repositories;
using DevEval.Domain.ValueObjects;
using NSubstitute;

namespace DevEval.Test.Application.Carts.Handlers
{
    public class ConvertCartToSaleHandlerTests
    {
        private readonly ICartRepository _cartRepositoryMock;
        private readonly IUserRepository _userRepositoryMock;
        private readonly ISaleRepository _saleRepositoryMock;
        private readonly ISaleEventPublisher _eventPublisherMock;
        private readonly ConvertCartToSaleHandler _handler;

        public ConvertCartToSaleHandlerTests()
        {
            _cartRepositoryMock = Substitute.For<ICartRepository>();
            _userRepositoryMock = Substitute.For<IUserRepository>();
            _saleRepositoryMock = Substitute.For<ISaleRepository>();
            _eventPublisherMock = Substitute.For<ISaleEventPublisher>();
            _handler = new ConvertCartToSaleHandler(
                _cartRepositoryMock,
                _userRepositoryMock,
                _saleRepositoryMock,
                _eventPublisherMock);
        }

        [Fact]
        public async Task Handle_ShouldConvertCartUsingRealUserData()
        {
            // Arrange
            var cart = new Cart(5);
            typeof(Cart).GetProperty(nameof(Cart.Id))?.SetValue(cart, 12);
            cart.AddProduct(new CartProduct(1, 10m, 4));

            var user = new User("john@example.com", "johndoe", "secret", UserRole.Customer);
            typeof(User).GetProperty(nameof(User.Id))?.SetValue(user, 5);
            user.UpdateName(new Name("John", "Doe"));
            user.UpdateAddress(new Address("Sao Paulo", "Paulista", 1000, "01310-100", Geolocation.Empty));

            _cartRepositoryMock.GetByIdWithProductsAsync(12).Returns(cart);
            _userRepositoryMock.GetByIdAsync(5).Returns(user);
            _saleRepositoryMock.AddAsync(Arg.Any<Sale>()).Returns(callInfo => callInfo.Arg<Sale>());

            // Act
            var result = await _handler.Handle(new ConvertCartToSaleCommand(12), CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("John Doe", result.Value.CustomerName);
            Assert.Equal("Sao Paulo", result.Value.BranchName);
            Assert.Single(result.Value.Items);
            Assert.Equal(36m, result.Value.TotalAmount);

            await _saleRepositoryMock.Received(1).AddAsync(Arg.Is<Sale>(sale =>
                sale.CustomerName == "John Doe" &&
                sale.BranchName == "Sao Paulo" &&
                sale.Items.Count == 1));
            await _eventPublisherMock.Received(1).PublishSaleCreatedAsync(Arg.Any<Sale>());
            await _cartRepositoryMock.Received(1).DeleteAsync(cart.Id);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var cart = new Cart(7);
            typeof(Cart).GetProperty(nameof(Cart.Id))?.SetValue(cart, 99);
            cart.AddProduct(new CartProduct(1, 10m, 1));

            _cartRepositoryMock.GetByIdWithProductsAsync(99).Returns(cart);
            _userRepositoryMock.GetByIdAsync(7).Returns((User?)null);

            // Act
            var result = await _handler.Handle(new ConvertCartToSaleCommand(99), CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);
            Assert.IsType<NotFoundError>(result.Errors.First());
            Assert.Equal("User with ID 7 not found.", result.Errors.First().Message);
            await _saleRepositoryMock.DidNotReceive().AddAsync(Arg.Any<Sale>());
            await _eventPublisherMock.DidNotReceive().PublishSaleCreatedAsync(Arg.Any<Sale>());
        }
    }
}
