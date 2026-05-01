using DevEval.Application.Sales.Commands;
using FluentValidation;

namespace DevEval.Application.Sales.Validators
{
    public class CreateSaleValidator : AbstractValidator<CreateSaleCommand>
    {
        public CreateSaleValidator()
        {
            RuleFor(command => command.SaleNumber)
                .NotEmpty().WithMessage("Sale number is required.")
                .Matches("^[A-Za-z0-9-]+$").WithMessage("Sale number must be alphanumeric and may include hyphens.");

            RuleFor(command => command.SaleDate)
                .NotEmpty().WithMessage("Sale date is required.")
                .LessThanOrEqualTo(DateTime.Now).WithMessage("Sale date cannot be in the future.");

            RuleFor(command => command.CustomerId)
                .NotEmpty().WithMessage("Customer ID is required.");

            RuleFor(command => command.CustomerName)
                .NotEmpty().WithMessage("Customer name is required.")
                .MaximumLength(100).WithMessage("Customer name must not exceed 100 characters.");

            RuleFor(command => command.BranchId)
                .NotEmpty().WithMessage("Branch ID is required.");

            RuleFor(command => command.BranchName)
                .NotEmpty().WithMessage("Branch name is required.")
                .MaximumLength(100).WithMessage("Branch name must not exceed 100 characters.");

            RuleFor(command => command.Items)
                .NotEmpty().WithMessage("At least one sale item is required.")
                .Must(items => items.All(item => item.Quantity > 0))
                .WithMessage("All sale items must have a quantity greater than zero.");

            RuleForEach(command => command.Items).SetValidator(new SaleItemValidator());
        }
    }
}
