using DevEval.Application.Common.Mappings;
using DevEval.Application.Sales.Dtos;
using DevEval.Application.Sales.Queries;
using DevEval.Common.Helpers.Pagination;
using DevEval.Domain.Repositories;
using FluentResults;
using MediatR;

namespace DevEval.Application.Users.Handlers
{
    public class GetSalesHandler : IRequestHandler<GetSalesQuery, Result<PaginatedResult<SaleDto>>>
    {
        private readonly ISaleRepository _repository;

        public GetSalesHandler(ISaleRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<PaginatedResult<SaleDto>>> Handle(GetSalesQuery request, CancellationToken cancellationToken)
        {
            var result = await _repository.GetAllAsync(request.Parameters);

            return Result.Ok(new PaginatedResult<SaleDto>
            {
                Items = result.Items.Select(sale => sale.ToDto()).ToList(),
                TotalItems = result.TotalItems,
                CurrentPage = result.CurrentPage,
                TotalPages = result.TotalPages
            });
        }
    }
}
