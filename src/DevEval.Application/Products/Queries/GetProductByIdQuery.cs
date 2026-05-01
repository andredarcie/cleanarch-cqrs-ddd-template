using DevEval.Application.Products.Dtos;
using FluentResults;
using MediatR;

namespace DevEval.Application.Products.Queries
{
    public class GetProductByIdQuery : IRequest<Result<ProductDto>>
    {
        public int Id { get; set; }

        public GetProductByIdQuery(int id)
        {
            Id = id;
        }
    }
}
