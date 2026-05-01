using DevEval.Application.Common.Mappings;
using DevEval.Application.Common.Errors;
using DevEval.Application.Products.Dtos;
using DevEval.Application.Products.Queries;
using DevEval.Domain.Repositories;
using FluentResults;
using MediatR;

namespace DevEval.Application.Products.Handlers
{
    public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
    {
        private readonly IProductRepository _repository;

        public GetProductByIdHandler(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _repository.GetByIdAsync(request.Id);

            if (product == null)
                return Result.Fail(new NotFoundError($"Product with ID {request.Id} not found."));

            return Result.Ok(product.ToDto());
        }
    }
}
