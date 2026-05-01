using MediatR;
using DevEval.Application.Sales.Dtos;
using DevEval.Application.Common.Mappings;
using DevEval.Application.Carts.Commands;
using DevEval.Application.Common.Errors;
using DevEval.Domain.Entities.Sale;
using DevEval.Domain.Repositories;
using FluentResults;

namespace DevEval.Application.Carts.Handlers
{
    public class ConvertCartToSaleHandler : IRequestHandler<ConvertCartToSaleCommand, Result<SaleDto>>
    {
        private readonly ICartRepository _cartRepository;
        private readonly ISaleRepository _saleRepository;

        public ConvertCartToSaleHandler(
            ICartRepository cartRepository,
            ISaleRepository saleRepository)
        {
            _cartRepository = cartRepository;
            _saleRepository = saleRepository;
        }

        public async Task<Result<SaleDto>> Handle(ConvertCartToSaleCommand request, CancellationToken cancellationToken)
        {
            var cart = await _cartRepository.GetByIdWithProductsAsync(request.CartId);

            if (cart == null)
                return Result.Fail(new NotFoundError("Cart not found."));

            if (cart.Products == null || !cart.Products.Any())
                return Result.Fail(new ValidationError("Cannot convert an empty cart to a sale."));

            var sale = Sale.FromCart(cart);
            var createdSale = await _saleRepository.AddAsync(sale);
            await _cartRepository.DeleteAsync(cart.Id);

            return Result.Ok(createdSale.ToDto());
        }
    }
}
