using DevEval.Application.Carts.Dtos;
using DevEval.Common.Helpers.Pagination;
using FluentResults;
using MediatR;

namespace DevEval.Application.Carts.Queries
{
    public class GetCartsQuery : IRequest<Result<PaginatedResult<CartDto>>>
    {
        public PaginationParameters Parameters { get; set; }
        public int? UserId { get; set; }
        public DateTime? MinDate { get; set; }
        public DateTime? MaxDate { get; set; }

        public GetCartsQuery(
            PaginationParameters parameters,
            int? userId = null,
            DateTime? minDate = null,
            DateTime? maxDate = null)
        {
            Parameters = parameters;
            UserId = userId;
            MinDate = minDate;
            MaxDate = maxDate;
        }
    }
}
