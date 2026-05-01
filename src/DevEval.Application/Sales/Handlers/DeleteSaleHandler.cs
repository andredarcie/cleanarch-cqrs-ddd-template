using DevEval.Application.Common.Errors;
using DevEval.Application.Sales.Commands;
using DevEval.Application.Sales.Services;
using DevEval.Domain.Repositories;
using FluentResults;
using MediatR;

namespace DevEval.Application.Sales.Handlers
{
    public class DeleteSaleHandler : IRequestHandler<DeleteSaleCommand, Result>
    {
        private readonly ISaleRepository _repository;
        private readonly ISaleEventPublisher _eventPublisher;

        public DeleteSaleHandler(ISaleRepository repository, ISaleEventPublisher eventPublisher)
        {
            _repository = repository;
            _eventPublisher = eventPublisher;
        }

        public async Task<Result> Handle(DeleteSaleCommand request, CancellationToken cancellationToken)
        {
            var sale = await _repository.GetByIdAsync(request.Id);
            if (sale == null)
                return Result.Fail(new NotFoundError($"Sale with ID {request.Id} not found."));

            sale.CancelSale();
            await _repository.UpdateAsync(sale);
            await _eventPublisher.PublishSaleCancelledAsync(sale, "Cancelled by user");

            return Result.Ok();
        }
    }
}
