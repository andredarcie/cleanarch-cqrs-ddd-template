using DevEval.Application.Products.Dtos;
using DevEval.Common.Helpers.Pagination;
using FluentResults;
using MediatR;

namespace DevEval.Application.Products.Queries
{
    public class GetProductsByCategoryQuery : IRequest<Result<PaginatedResult<ProductDto>>>
    {
        public string Category { get; set; }
        public PaginationParameters Parameters { get; set; }

        public GetProductsByCategoryQuery(string category, PaginationParameters parameters)
        {
            Category = category;
            Parameters = parameters;
        }
    }
}
