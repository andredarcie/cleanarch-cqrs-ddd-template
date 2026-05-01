using DevEval.Application.Carts.Dtos;
using DevEval.Common.Helpers.Pagination;
using FluentResults;
using MediatR;

namespace DevEval.Application.Carts.Queries
{
    public class GetCartsQuery : IRequest<Result<PaginatedResult<CartDto>>>
    {
        public PaginationParameters Parameters { get; set; }

        public GetCartsQuery(PaginationParameters parameters)
        {
            Parameters = parameters;
        }
    }
}
