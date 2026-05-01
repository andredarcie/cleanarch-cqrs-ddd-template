using DevEval.Application.Carts.Commands;
using DevEval.Application.Carts.Dtos;
using DevEval.Domain.Entities.Cart;

namespace DevEval.Application.Common.Mappings;

internal static class CartMappings
{
    public static Cart ToEntity(this CreateCartCommand command)
    {
        return new Cart(
            command.UserId,
            command.Date == default ? DateTime.UtcNow : command.Date,
            command.Products.Select(product => product.ToEntity()).ToList());
    }

    public static void ApplyTo(this UpdateCartCommand command, Cart cart)
    {
        cart.UpdateUserId(command.UserId);
        cart.UpdateDate(DateTime.Parse(command.Date));
        cart.Products = command.Products.Select(product => product.ToEntity()).ToList();
    }

    public static CartDto ToDto(this Cart cart)
    {
        return new CartDto
        {
            Id = cart.Id,
            UserId = cart.UserId,
            Date = cart.Date.ToString("yyyy-MM-dd"),
            Products = cart.Products.Select(product => product.ToDto()).ToList()
        };
    }

    public static CartProduct ToEntity(this CartProductDto dto)
    {
        return new CartProduct(dto.ProductId, dto.UnitPrice, dto.Quantity);
    }

    public static CartProductDto ToDto(this CartProduct product)
    {
        return new CartProductDto
        {
            ProductId = product.ProductId,
            UnitPrice = product.UnitPrice,
            Quantity = product.Quantity
        };
    }
}
