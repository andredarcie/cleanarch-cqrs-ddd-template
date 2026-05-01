using DevEval.Application.Common.Mappings;
using DevEval.Application.Common.Errors;
using DevEval.Application.Sales.Commands;
using DevEval.Application.Sales.Dtos;
using DevEval.Application.Sales.Services;
using DevEval.Domain.Repositories;
using FluentResults;
using MediatR;

namespace DevEval.Application.Sales.Handlers
{
    public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, Result<SaleDto>>
    {
        private readonly ISaleRepository _repository;
        private readonly ISaleEventPublisher _eventPublisher;

        public UpdateSaleHandler(ISaleRepository repository, ISaleEventPublisher eventPublisher)
        {
            _repository = repository;
            _eventPublisher = eventPublisher;
        }

        public async Task<Result<SaleDto>> Handle(UpdateSaleCommand request, CancellationToken cancellationToken)
        {
            var existingSale = await _repository.GetByIdAsync(request.Id);

            if (existingSale == null)
                return Result.Fail(new NotFoundError($"Sale with ID {request.Id} not found."));

            request.ApplyTo(existingSale);

            var updatedSale = await _repository.UpdateAsync(existingSale);
            await _eventPublisher.PublishSaleModifiedAsync(updatedSale);

            return Result.Ok(updatedSale.ToDto());
        }
    }
}
