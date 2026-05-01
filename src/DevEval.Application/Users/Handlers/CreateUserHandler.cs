using DevEval.Application.Users.Commands;
using DevEval.Application.Common.Mappings;
using DevEval.Application.Users.Dtos;
using DevEval.Common.Services;
using DevEval.Domain.Repositories;
using FluentResults;
using MediatR;

namespace DevEval.Application.Users.Handlers
{
    public class CreateUserHandler : IRequestHandler<CreateUserCommand, Result<UserDto>>
    {
        private readonly IUserRepository _repository;
        private readonly IPasswordService _passwordService;

        public CreateUserHandler(IUserRepository repository, IPasswordService passwordService)
        {
            _repository = repository;
            _passwordService = passwordService;
        }

        public async Task<Result<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            request.Password = _passwordService.HashPassword(request.Password);

            var user = request.ToEntity();
            var createdUser = await _repository.AddAsync(user);

            return Result.Ok(createdUser.ToDto());
        }
    }
}
