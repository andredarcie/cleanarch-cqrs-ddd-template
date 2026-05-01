using MediatR;
using DevEval.Application.Sales.Dtos;
using DevEval.Application.Common.Mappings;
using DevEval.Application.Carts.Commands;
using DevEval.Application.Common.Errors;
using DevEval.Application.Sales.Services;
using DevEval.Domain.Repositories;
using FluentResults;

namespace DevEval.Application.Carts.Handlers
{
    public class ConvertCartToSaleHandler : IRequestHandler<ConvertCartToSaleCommand, Result<SaleDto>>
    {
        private readonly ICartRepository _cartRepository;
        private readonly IUserRepository _userRepository;
        private readonly ISaleRepository _saleRepository;
        private readonly ISaleEventPublisher _eventPublisher;

        public ConvertCartToSaleHandler(
            ICartRepository cartRepository,
            IUserRepository userRepository,
            ISaleRepository saleRepository,
            ISaleEventPublisher eventPublisher)
        {
            _cartRepository = cartRepository;
            _userRepository = userRepository;
            _saleRepository = saleRepository;
            _eventPublisher = eventPublisher;
        }

        public async Task<Result<SaleDto>> Handle(ConvertCartToSaleCommand request, CancellationToken cancellationToken)
        {
            var cart = await _cartRepository.GetByIdWithProductsAsync(request.CartId);

            if (cart == null)
                return Result.Fail(new NotFoundError("Cart not found."));

            if (cart.Products == null || !cart.Products.Any())
                return Result.Fail(new ValidationError("Cannot convert an empty cart to a sale."));

            var user = await _userRepository.GetByIdAsync(cart.UserId);

            if (user == null)
                return Result.Fail(new NotFoundError($"User with ID {cart.UserId} not found."));

            var sale = Domain.Entities.Sale.Sale.FromCart(
                cart,
                CreateExternalIdentity($"user:{user.Id}"),
                ResolveCustomerName(user),
                CreateExternalIdentity($"branch:{ResolveBranchName(user)}"),
                ResolveBranchName(user));

            var createdSale = await _saleRepository.AddAsync(sale);
            await _eventPublisher.PublishSaleCreatedAsync(createdSale);
            await _cartRepository.DeleteAsync(cart.Id);

            return Result.Ok(createdSale.ToDto());
        }

        private static Guid CreateExternalIdentity(string value)
        {
            var bytes = System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(value));
            Array.Resize(ref bytes, 16);

            return new Guid(bytes);
        }

        private static string ResolveCustomerName(Domain.Entities.User.User user)
        {
            if (user.Name != null)
            {
                return user.Name.FullName;
            }

            return user.Username;
        }

        private static string ResolveBranchName(Domain.Entities.User.User user)
        {
            if (!string.IsNullOrWhiteSpace(user.Address?.City))
            {
                return user.Address.City;
            }

            return "Online Store";
        }
    }
}
