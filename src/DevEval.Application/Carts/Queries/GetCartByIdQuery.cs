using DevEval.Application.Carts.Dtos;
using FluentResults;
using MediatR;

namespace DevEval.Application.Carts.Queries
{
    public class GetCartByIdQuery : IRequest<Result<CartDto>>
    {
        public int Id { get; set; }

        public GetCartByIdQuery(int id)
        {
            Id = id;
        }
    }
}
