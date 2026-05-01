using FluentResults;
using MediatR;

namespace DevEval.Application.Carts.Commands
{
    public class DeleteCartCommand : IRequest<Result>
    {
        public DeleteCartCommand(int id)
        {
            Id = id;
        }

        public int Id { get; set; }
    }
}
