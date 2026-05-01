using FluentResults;
using MediatR;

namespace DevEval.Application.Users.Commands
{
    public class DeleteUserCommand : IRequest<Result>
    {
        public DeleteUserCommand(int id)
        {
            Id = id;
        }

        public int Id { get; set; }
    }
}
