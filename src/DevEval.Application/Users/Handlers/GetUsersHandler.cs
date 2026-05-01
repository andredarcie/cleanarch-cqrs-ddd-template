using DevEval.Application.Common.Mappings;
using DevEval.Application.Users.Dtos;
using DevEval.Application.Users.Queries;
using DevEval.Common.Helpers.Pagination;
using DevEval.Domain.Repositories;
using FluentResults;
using MediatR;

namespace DevEval.Application.Users.Handlers
{
    public class GetUsersHandler : IRequestHandler<GetUsersQuery, Result<PaginatedResult<UserDto>>>
    {
        private readonly IUserRepository _repository;

        public GetUsersHandler(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<PaginatedResult<UserDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            var result = await _repository.GetFilteredAsync(
                request.Username,
                request.Email,
                request.Status,
                request.Role,
                request.Parameters);

            return Result.Ok(new PaginatedResult<UserDto>
            {
                Data = result.Data.Select(user => user.ToDto()).ToList(),
                TotalItems = result.TotalItems,
                CurrentPage = result.CurrentPage,
                TotalPages = result.TotalPages
            });
        }
    }
}
