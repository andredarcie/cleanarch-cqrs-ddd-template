using DevEval.Application.Users.Dtos;
using DevEval.Common.Helpers.Pagination;
using FluentResults;
using MediatR;

namespace DevEval.Application.Users.Queries
{
    public class GetUsersQuery : IRequest<Result<PaginatedResult<UserDto>>>
    {
        public PaginationParameters Parameters { get; set; }

        public GetUsersQuery(PaginationParameters parameters)
        {
            Parameters = parameters;
        }
    }
}
