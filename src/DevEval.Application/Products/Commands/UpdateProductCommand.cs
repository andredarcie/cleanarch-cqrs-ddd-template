using DevEval.Application.Products.Dtos;
using FluentResults;
using MediatR;

namespace DevEval.Application.Products.Commands
{
    public class UpdateProductCommand : IRequest<Result<ProductDto>>
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public decimal Price { get; set; }
        public required string Description { get; set; }
        public required string Category { get; set; }
        public required string Image { get; set; }
        public RatingDto? Rating { get; set; }
    }
}
