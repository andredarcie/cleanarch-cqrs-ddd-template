using DevEval.Application.Common.Mappings;
using DevEval.Application.Common.Errors;
using DevEval.Application.Users.Commands;
using DevEval.Application.Users.Dtos;
using DevEval.Common.Services;
using DevEval.Domain.Repositories;
using FluentResults;
using MediatR;

namespace DevEval.Application.Users.Handlers
{
    public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, Result<UserDto>>
    {
        private readonly IUserRepository _repository;
        private readonly IPasswordService _passwordService;

        public UpdateUserHandler(IUserRepository repository, IPasswordService passwordService)
        {
            _repository = repository;
            _passwordService = passwordService;
        }

        public async Task<Result<UserDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await _repository.GetByIdAsync(request.Id);

            if (existingUser == null)
                return Result.Fail(new NotFoundError($"User with ID {request.Id} not found."));

            if (!string.IsNullOrEmpty(request.Password))
                request.Password = _passwordService.HashPassword(request.Password);

            request.ApplyTo(existingUser);

            var updatedUser = await _repository.UpdateAsync(existingUser);

            return Result.Ok(updatedUser.ToDto());
        }
    }
}
