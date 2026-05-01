using DevEval.Application.Products.Dtos;
using DevEval.Common.Helpers.Pagination;
using FluentResults;
using MediatR;

namespace DevEval.Application.Products.Queries
{
    public class GetProductsQuery : IRequest<Result<PaginatedResult<ProductDto>>>
    {
        public PaginationParameters Parameters { get; set; }
        public string? Title { get; set; }
        public string? Category { get; set; }
        public decimal? Price { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        public GetProductsQuery(
            PaginationParameters parameters,
            string? title = null,
            string? category = null,
            decimal? price = null,
            decimal? minPrice = null,
            decimal? maxPrice = null)
        {
            Parameters = parameters;
            Title = title;
            Category = category;
            Price = price;
            MinPrice = minPrice;
            MaxPrice = maxPrice;
        }
    }
}
