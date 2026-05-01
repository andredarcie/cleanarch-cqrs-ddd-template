using DevEval.Application.Sales.Dtos;
using FluentResults;
using MediatR;

namespace DevEval.Application.Sales.Commands
{
    public class UpdateSaleCommand : IRequest<Result<SaleDto>>
    {
        public Guid Id { get; set; }
        public required string SaleNumber { get; set; }
        public DateTime SaleDate { get; set; }
        public Guid CustomerId { get; set; }
        public required string CustomerName { get; set; }
        public Guid BranchId { get; set; }
        public required string BranchName { get; set; }
        public required List<SaleItemDto> Items { get; set; }
    }
}
