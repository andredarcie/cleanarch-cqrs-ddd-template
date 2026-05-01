using DevEval.Application.Sales.Dtos;
using FluentResults;
using MediatR;

namespace DevEval.Application.Sales.Queries
{
    public class GetSaleByIdQuery : IRequest<Result<SaleDto>>
    {
        public Guid Id { get; set; }

        public GetSaleByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}