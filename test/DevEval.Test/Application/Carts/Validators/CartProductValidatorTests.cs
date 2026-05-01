using DevEval.Application.Carts.Dtos;
using DevEval.Application.Carts.Validators;

namespace DevEval.Test.Application.Carts.Validators
{
    public class CartProductValidatorTests
    {
        private readonly CartProductValidator _validator = new();

        [Fact]
        public void Should_Pass_When_Quantity_Is_Valid()
        {
            var dto = new CartProductDto { ProductId = 1, Quantity = 10 };

            var result = _validator.Validate(dto);

            Assert.True(result.IsValid);
        }

        [Fact]
        public void Should_Pass_When_Quantity_Is_Exactly_20()
        {
            var dto = new CartProductDto { ProductId = 1, Quantity = 20 };

            var result = _validator.Validate(dto);

            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData(21)]
        [InlineData(100)]
        public void Should_Fail_When_Quantity_Exceeds_20(int quantity)
        {
            var dto = new CartProductDto { ProductId = 1, Quantity = quantity };

            var result = _validator.Validate(dto);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Quantity" && e.ErrorMessage == "Product quantity cannot exceed 20.");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_Fail_When_Quantity_Is_Zero_Or_Negative(int quantity)
        {
            var dto = new CartProductDto { ProductId = 1, Quantity = quantity };

            var result = _validator.Validate(dto);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Quantity");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_Fail_When_ProductId_Is_Zero_Or_Negative(int productId)
        {
            var dto = new CartProductDto { ProductId = productId, Quantity = 1 };

            var result = _validator.Validate(dto);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "ProductId");
        }
    }
}
