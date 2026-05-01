using DevEval.Application.Carts.Commands;
using DevEval.Application.Common.Mappings;
using DevEval.Application.Carts.Dtos;
using DevEval.Application.Common.Errors;
using DevEval.Domain.Repositories;
using FluentResults;
using MediatR;

namespace DevEval.Application.Carts.Handlers
{
    public class UpdateCartHandler : IRequestHandler<UpdateCartCommand, Result<CartDto>>
    {
        private readonly ICartRepository _repository;

        public UpdateCartHandler(ICartRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<CartDto>> Handle(UpdateCartCommand request, CancellationToken cancellationToken)
        {
            var existingCart = await _repository.GetByIdAsync(request.Id);

            if (existingCart == null)
                return Result.Fail(new NotFoundError($"Cart with ID {request.Id} not found."));

            request.ApplyTo(existingCart);

            var updatedCart = await _repository.UpdateAsync(existingCart);

            return Result.Ok(updatedCart.ToDto());
        }
    }
}
