using AutoMapper;
using DevEval.Application.Common.Errors;
using DevEval.Application.Products.Dtos;
using DevEval.Application.Products.Queries;
using DevEval.Domain.Repositories;
using FluentResults;
using MediatR;

namespace DevEval.Application.Products.Handlers
{
    public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
    {
        private readonly IProductRepository _repository;
        private readonly IMapper _mapper;

        public GetProductByIdHandler(IProductRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _repository.GetByIdAsync(request.Id);

            if (product == null)
                return Result.Fail(new NotFoundError($"Product with ID {request.Id} not found."));

            return Result.Ok(_mapper.Map<ProductDto>(product));
        }
    }
}
