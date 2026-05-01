using DevEval.Application.Common.Mappings;
using DevEval.Application.Products.Commands;
using DevEval.Application.Products.Dtos;
using DevEval.Domain.Repositories;
using FluentResults;
using MediatR;

namespace DevEval.Application.Products.Handlers
{
    public class CreateProductHandler : IRequestHandler<CreateProductCommand, Result<ProductDto>>
    {
        private readonly IProductRepository _repository;

        public CreateProductHandler(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<ProductDto>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = request.ToEntity();
            var createdProduct = await _repository.AddAsync(product);
            return Result.Ok(createdProduct.ToDto());
        }
    }
}
