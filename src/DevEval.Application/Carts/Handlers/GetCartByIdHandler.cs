using AutoMapper;
using DevEval.Application.Carts.Dtos;
using DevEval.Application.Carts.Queries;
using DevEval.Application.Common.Errors;
using DevEval.Domain.Repositories;
using FluentResults;
using MediatR;

namespace DevEval.Application.Carts.Handlers
{
    public class GetCartByIdHandler : IRequestHandler<GetCartByIdQuery, Result<CartDto>>
    {
        private readonly ICartRepository _repository;
        private readonly IMapper _mapper;

        public GetCartByIdHandler(ICartRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<CartDto>> Handle(GetCartByIdQuery request, CancellationToken cancellationToken)
        {
            var cart = await _repository.GetByIdAsync(request.Id);

            if (cart == null)
                return Result.Fail(new NotFoundError($"Cart with ID {request.Id} not found."));

            return Result.Ok(_mapper.Map<CartDto>(cart));
        }
    }
}
