using AutoMapper;
using DevEval.Application.Sales.Commands;
using DevEval.Application.Sales.Dtos;
using DevEval.Application.Sales.Services;
using DevEval.Domain.Entities.Sale;
using DevEval.Domain.Repositories;
using FluentResults;
using MediatR;

namespace DevEval.Application.Sales.Handlers
{
    public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, Result<SaleDto>>
    {
        private readonly ISaleRepository _repository;
        private readonly IMapper _mapper;
        private readonly ISaleEventPublisher _eventPublisher;

        public CreateSaleHandler(ISaleRepository repository, IMapper mapper, ISaleEventPublisher eventPublisher)
        {
            _repository = repository;
            _mapper = mapper;
            _eventPublisher = eventPublisher;
        }

        public async Task<Result<SaleDto>> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
        {
            var sale = _mapper.Map<Sale>(request);
            var createdSale = await _repository.AddAsync(sale);
            await _eventPublisher.PublishSaleCreatedAsync(createdSale);
            return Result.Ok(_mapper.Map<SaleDto>(createdSale));
        }
    }
}
