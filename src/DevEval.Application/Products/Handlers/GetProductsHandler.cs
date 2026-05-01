using DevEval.Application.Common.Mappings;
using DevEval.Application.Products.Dtos;
using DevEval.Application.Products.Queries;
using DevEval.Common.Helpers.Pagination;
using DevEval.Domain.Repositories;
using FluentResults;
using MediatR;

namespace DevEval.Application.Products.Handlers
{
    public class GetProductsHandler : IRequestHandler<GetProductsQuery, Result<PaginatedResult<ProductDto>>>
    {
        private readonly IProductRepository _repository;

        public GetProductsHandler(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<PaginatedResult<ProductDto>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            var result = await _repository.GetFilteredAsync(
                request.Title,
                request.Category,
                request.Price,
                request.MinPrice,
                request.MaxPrice,
                request.Parameters);

            return Result.Ok(new PaginatedResult<ProductDto>
            {
                Data = result.Data.Select(product => product.ToDto()).ToList(),
                TotalItems = result.TotalItems,
                CurrentPage = result.CurrentPage,
                TotalPages = result.TotalPages
            });
        }
    }
}
