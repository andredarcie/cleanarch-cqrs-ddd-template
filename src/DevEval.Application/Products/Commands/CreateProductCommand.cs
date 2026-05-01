using DevEval.Application.Products.Dtos;
using FluentResults;
using MediatR;

namespace DevEval.Application.Products.Commands
{
    public class CreateProductCommand : IRequest<Result<ProductDto>>
    {
        /// <summary>
        /// Gets or sets the title of the product.
        /// </summary>
        public required string Title { get; set; }

        /// <summary>
        /// Gets or sets the price of the product.
        /// </summary>
        public required decimal Price { get; set; }

        /// <summary>
        /// Gets or sets the description of the product.
        /// </summary>
        public required string Description { get; set; }

        /// <summary>
        /// Gets or sets the category of the product.
        /// </summary>
        public required string Category { get; set; }

        /// <summary>
        /// Gets or sets the URL of the product image.
        /// </summary>
        public required string Image { get; set; }

        /// <summary>
        /// Gets or sets the rating information for the product.
        /// </summary>
        public RatingDto? Rating { get; set; }
    }
}
