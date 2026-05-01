using DevEval.Application.Sales.Dtos;
using DevEval.Common.Helpers.Pagination;
using FluentResults;
using MediatR;

namespace DevEval.Application.Sales.Queries
{
    public class GetSalesQuery : IRequest<Result<PaginatedResult<SaleDto>>>
    {
        public PaginationParameters Parameters { get; set; }

        public GetSalesQuery(PaginationParameters parameters)
        {
            Parameters = parameters;
        }
    }
}
