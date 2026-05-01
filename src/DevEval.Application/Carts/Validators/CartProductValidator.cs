using FluentValidation;
using DevEval.Application.Carts.Dtos;

namespace DevEval.Application.Carts.Validators
{
    public class CartProductValidator : AbstractValidator<CartProductDto>
    {
        public CartProductValidator()
        {
            RuleFor(product => product.ProductId)
                .GreaterThan(0).WithMessage("Product ID must be greater than zero.");

            RuleFor(product => product.Quantity)
                .GreaterThan(0).WithMessage("Product quantity must be greater than zero.")
                .LessThanOrEqualTo(20).WithMessage("Product quantity cannot exceed 20.");
        }
    }
}