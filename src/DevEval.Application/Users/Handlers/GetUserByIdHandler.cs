using AutoMapper;
using DevEval.Application.Common.Errors;
using DevEval.Application.Users.Dtos;
using DevEval.Application.Users.Queries;
using DevEval.Domain.Repositories;
using FluentResults;
using MediatR;

namespace DevEval.Application.Users.Handlers
{
    public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;

        public GetUserByIdHandler(IUserRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _repository.GetByIdAsync(request.Id);

            if (user == null)
                return Result.Fail(new NotFoundError($"User with ID {request.Id} not found."));

            return Result.Ok(_mapper.Map<UserDto>(user));
        }
    }
}
