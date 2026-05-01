using DevEval.Application.Products.Commands;
using DevEval.Application.Products.Dtos;
using DevEval.Domain.Entities.Product;
using DevEval.Domain.ValueObjects;

namespace DevEval.Application.Common.Mappings;

internal static class ProductMappings
{
    public static Product ToEntity(this CreateProductCommand command)
    {
        var product = new Product(command.Title, command.Price, command.Description, command.Image, command.Category);
        product.UpdateRating(command.Rating.ToValueObject());
        return product;
    }

    public static void ApplyTo(this UpdateProductCommand command, Product product)
    {
        product.UpdateTitle(command.Title);
        product.UpdatePrice(command.Price);
        product.UpdateDescription(command.Description);
        product.UpdateCategory(command.Category);
        product.UpdateImage(command.Image);
        product.UpdateRating(command.Rating.ToValueObject());
    }

    public static ProductDto ToDto(this Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Title = product.Title,
            Price = product.Price,
            Description = product.Description,
            Category = product.Category,
            Image = product.Image,
            Rating = product.Rating.ToDto()
        };
    }

    private static Rating ToValueObject(this RatingDto? rating)
    {
        return rating is null ? Rating.Empty : new Rating(rating.Rate, rating.Count);
    }

    private static RatingDto ToDto(this Rating? rating)
    {
        return rating is null
            ? new RatingDto()
            : new RatingDto
            {
                Rate = rating.Rate,
                Count = rating.Count
            };
    }
}
