using AutoMapper;
using DevEval.Application.Common.Errors;
using DevEval.Application.Sales.Dtos;
using DevEval.Application.Sales.Queries;
using DevEval.Domain.Repositories;
using FluentResults;
using MediatR;

namespace DevEval.Application.Sales.Handlers
{
    public class GetSaleByIdHandler : IRequestHandler<GetSaleByIdQuery, Result<SaleDto>>
    {
        private readonly ISaleRepository _repository;
        private readonly IMapper _mapper;

        public GetSaleByIdHandler(ISaleRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<SaleDto>> Handle(GetSaleByIdQuery request, CancellationToken cancellationToken)
        {
            var sale = await _repository.GetByIdAsync(request.Id);

            if (sale == null)
                return Result.Fail(new NotFoundError($"Sale with ID {request.Id} not found."));

            return Result.Ok(_mapper.Map<SaleDto>(sale));
        }
    }
}
