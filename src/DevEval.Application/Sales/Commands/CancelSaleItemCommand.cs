using FluentResults;
using MediatR;

namespace DevEval.Application.Sales.Commands
{
    public class CancelSaleItemCommand : IRequest<Result>
    {
        public Guid SaleId { get; set; }
        public Guid ItemId { get; set; }
        public string Reason { get; set; }

        public CancelSaleItemCommand(Guid saleId, Guid itemId, string reason)
        {
            SaleId = saleId;
            ItemId = itemId;
            Reason = reason;
        }
    }
}
