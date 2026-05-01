using AutoMapper;
using DevEval.Application.Carts.Commands;
using DevEval.Application.Carts.Dtos;
using DevEval.Application.Common.Errors;
using DevEval.Domain.Entities.Cart;
using DevEval.Domain.Repositories;
using FluentResults;
using MediatR;

namespace DevEval.Application.Carts.Handlers
{
    public class CreateCartHandler : IRequestHandler<CreateCartCommand, Result<CartDto>>
    {
        private readonly ICartRepository _repository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public CreateCartHandler(ICartRepository repository, IMapper mapper, IProductRepository productRepository)
        {
            _repository = repository;
            _mapper = mapper;
            _productRepository = productRepository;
        }

        public async Task<Result<CartDto>> Handle(CreateCartCommand request, CancellationToken cancellationToken)
        {
            request.Date = DateTime.UtcNow;

            var cart = _mapper.Map<Cart>(request);

            foreach (var cartProduct in cart.Products.Where(product => product != null))
            {
                var product = await _productRepository.GetByIdAsync(cartProduct.ProductId);

                if (product == null)
                    return Result.Fail(new ValidationError($"Product with ID {cartProduct.ProductId} does not exist."));
            }

            var createdCart = await _repository.AddAsync(cart);
            return Result.Ok(_mapper.Map<CartDto>(createdCart));
        }
    }
}
