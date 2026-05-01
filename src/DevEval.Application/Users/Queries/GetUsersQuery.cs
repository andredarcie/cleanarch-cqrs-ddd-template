using DevEval.Application.Users.Dtos;
using DevEval.Common.Helpers.Pagination;
using DevEval.Domain.Enums;
using FluentResults;
using MediatR;

namespace DevEval.Application.Users.Queries
{
    public class GetUsersQuery : IRequest<Result<PaginatedResult<UserDto>>>
    {
        public PaginationParameters Parameters { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public UserStatus? Status { get; set; }
        public UserRole? Role { get; set; }

        public GetUsersQuery(
            PaginationParameters parameters,
            string? username = null,
            string? email = null,
            UserStatus? status = null,
            UserRole? role = null)
        {
            Parameters = parameters;
            Username = username;
            Email = email;
            Status = status;
            Role = role;
        }
    }
}
