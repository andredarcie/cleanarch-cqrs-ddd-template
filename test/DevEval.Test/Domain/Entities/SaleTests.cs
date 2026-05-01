using DevEval.Domain.Entities.Sale;
using DevEval.Domain.Entities.Cart;

namespace DevEval.Test.Domain.Entities
{
    public class SaleTests
    {
        [Fact]
        public void Should_Create_Sale_With_Valid_Values()
        {
            // Arrange
            string saleNumber = "SALE-001";
            Guid customerId = Guid.NewGuid();
            string customerName = "John Doe";
            Guid branchId = Guid.NewGuid();
            string branchName = "Main Branch";

            // Act
            var sale = new Sale(saleNumber, customerId, customerName, branchId, branchName);

            // Assert
            Assert.Equal(saleNumber, sale.SaleNumber);
            Assert.Equal(customerId, sale.CustomerId);
            Assert.Equal(customerName, sale.CustomerName);
            Assert.Equal(branchId, sale.BranchId);
            Assert.Equal(branchName, sale.BranchName);
            Assert.False(sale.IsCancelled);
            Assert.Empty(sale.Items);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Should_Throw_Exception_When_Creating_Sale_With_Invalid_SaleNumber(string invalidSaleNumber)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new Sale(invalidSaleNumber, Guid.NewGuid(), "John Doe", Guid.NewGuid(), "Main Branch"));
        }

        [Fact]
        public void Should_Throw_Exception_When_Creating_Sale_With_Invalid_CustomerId()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new Sale("SALE-001", Guid.Empty, "John Doe", Guid.NewGuid(), "Main Branch"));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Should_Throw_Exception_When_Creating_Sale_With_Invalid_CustomerName(string invalidCustomerName)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new Sale("SALE-001", Guid.NewGuid(), invalidCustomerName, Guid.NewGuid(), "Main Branch"));
        }

        [Fact]
        public void Should_Throw_Exception_When_Creating_Sale_With_Invalid_BranchId()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new Sale("SALE-001", Guid.NewGuid(), "John Doe", Guid.Empty, "Main Branch"));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Should_Throw_Exception_When_Creating_Sale_With_Invalid_BranchName(string invalidBranchName)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new Sale("SALE-001", Guid.NewGuid(), "John Doe", Guid.NewGuid(), invalidBranchName));
        }

        [Fact]
        public void Should_Add_Item_To_Sale()
        {
            // Arrange
            var sale = new Sale("SALE-001", Guid.NewGuid(), "John Doe", Guid.NewGuid(), "Main Branch");

            // Act
            sale.AddItem(1, 2, 50.0m);

            // Assert
            Assert.Single(sale.Items);
            Assert.Equal(100.0m, sale.TotalAmount);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_Throw_Exception_When_Adding_Item_With_Invalid_Quantity(int invalidQuantity)
        {
            // Arrange
            var sale = new Sale("SALE-001", Guid.NewGuid(), "John Doe", Guid.NewGuid(), "Main Branch");

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => sale.AddItem(1, invalidQuantity, 50.0m));
        }

        [Fact]
        public void Should_Throw_Exception_When_Adding_Item_Above_Maximum_Quantity()
        {
            // Arrange
            var sale = new Sale("SALE-001", Guid.NewGuid(), "John Doe", Guid.NewGuid(), "Main Branch");

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => sale.AddItem(1, 21, 50.0m));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public void Should_Throw_Exception_When_Adding_Item_With_Invalid_UnitPrice(decimal invalidUnitPrice)
        {
            // Arrange
            var sale = new Sale("SALE-001", Guid.NewGuid(), "John Doe", Guid.NewGuid(), "Main Branch");

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => sale.AddItem(1, 2, invalidUnitPrice));
        }

        [Fact]
        public void Should_Cancel_Sale()
        {
            // Arrange
            var sale = new Sale("SALE-001", Guid.NewGuid(), "John Doe", Guid.NewGuid(), "Main Branch");
            sale.AddItem(1, 2, 50.0m);
            sale.AddItem(2, 1, 30.0m);

            // Act
            sale.CancelSale();

            // Assert
            Assert.True(sale.IsCancelled);
            Assert.All(sale.Items, item => Assert.True(item.IsCancelled));
        }

        [Fact]
        public void Should_Create_Sale_From_Cart()
        {
            // Arrange
            var cart = new Cart(1);
            cart.AddProduct(new CartProduct(1, 50.0m, 2));
            cart.AddProduct(new CartProduct(2, 30.0m, 1));
            var customerId = Guid.NewGuid();
            const string customerName = "John Doe";
            var branchId = Guid.NewGuid();
            const string branchName = "Main Branch";

            // Act
            var sale = Sale.FromCart(cart, customerId, customerName, branchId, branchName);

            // Assert
            Assert.NotNull(sale);
            Assert.Equal(2, sale.Items.Count);
            Assert.Equal(130.0m, sale.TotalAmount);
            Assert.Equal(customerId, sale.CustomerId);
            Assert.Equal(customerName, sale.CustomerName);
            Assert.Equal(branchId, sale.BranchId);
            Assert.Equal(branchName, sale.BranchName);
        }

        [Fact]
        public void Should_Throw_Exception_When_Creating_Sale_From_Null_Cart()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => Sale.FromCart(null, Guid.NewGuid(), "John Doe", Guid.NewGuid(), "Main Branch"));
        }
    }
}
