using DevEval.Application.Users.Dtos;
using FluentResults;
using MediatR;

namespace DevEval.Application.Users.Queries
{
    public class GetUserByIdQuery : IRequest<Result<UserDto>>
    {
        public int Id { get; set; }

        public GetUserByIdQuery(int id)
        {
            Id = id;
        }
    }
}
