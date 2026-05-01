using DevEval.Application.Common.Mappings;
using DevEval.Application.Products.Dtos;
using DevEval.Application.Products.Queries;
using DevEval.Common.Helpers.Pagination;
using DevEval.Domain.Repositories;
using FluentResults;
using MediatR;

namespace DevEval.Application.Products.Handlers
{
    public class GetProductsByCategoryHandler : IRequestHandler<GetProductsByCategoryQuery, Result<PaginatedResult<ProductDto>>>
    {
        private readonly IProductRepository _repository;

        public GetProductsByCategoryHandler(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<PaginatedResult<ProductDto>>> Handle(GetProductsByCategoryQuery request, CancellationToken cancellationToken)
        {
            var products = await _repository.GetProductsByCategoryAsync(request.Category, request.Parameters);

            return Result.Ok(new PaginatedResult<ProductDto>
            {
                Data = products.Data.Select(product => product.ToDto()).ToList(),
                TotalItems = products.TotalItems,
                CurrentPage = products.CurrentPage,
                TotalPages = products.TotalPages
            });
        }
    }
}
