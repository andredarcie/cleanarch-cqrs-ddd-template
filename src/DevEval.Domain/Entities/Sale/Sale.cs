namespace DevEval.Domain.Entities.Sale
{
    /// <summary>
    /// Represents a sale transaction with details and associated metadata.
    /// </summary>
    public class Sale
    {
        /// <summary>
        /// Gets the unique identifier of the sale.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Gets the sale number.
        /// </summary>
        public string SaleNumber { get; private set; }

        /// <summary>
        /// Gets the date when the sale was created.
        /// </summary>
        public DateTime SaleDate { get; private set; }

        /// <summary>
        /// Indicates whether the sale has been cancelled.
        /// </summary>
        public bool IsCancelled { get; private set; }

        /// <summary>
        /// Gets the customer ID associated with the sale.
        /// </summary>
        public Guid CustomerId { get; private set; }

        /// <summary>
        /// Gets the customer name associated with the sale.
        /// </summary>
        public string CustomerName { get; private set; }

        /// <summary>
        /// Gets the branch ID where the sale occurred.
        /// </summary>
        public Guid BranchId { get; private set; }

        /// <summary>
        /// Gets the branch name where the sale occurred.
        /// </summary>
        public string BranchName { get; private set; }

        /// <summary>
        /// Gets the list of items in the sale.
        /// </summary>
        public List<SaleItem> Items { get; set; } = new List<SaleItem>();

        /// <summary>
        /// Gets the total amount of the sale, calculated from the items.
        /// </summary>
        public decimal TotalAmount => Items.Sum(item => item.TotalPrice);

        /// <summary>
        /// Initializes a new instance of the <see cref="Sale"/> class.
        /// </summary>
        /// <param name="saleNumber">The unique sale number.</param>
        /// <param name="customerId">The customer ID associated with the sale.</param>
        /// <param name="customerName">The customer name associated with the sale.</param>
        /// <param name="branchId">The branch ID where the sale occurred.</param>
        /// <param name="branchName">The branch name where the sale occurred.</param>
        public Sale(string saleNumber, Guid customerId, string customerName, Guid branchId, string branchName)
        {
            if (string.IsNullOrWhiteSpace(saleNumber))
                throw new ArgumentException("Sale number cannot be null or empty.", nameof(saleNumber));

            if (customerId == Guid.Empty)
                throw new ArgumentException("Customer ID must be a valid GUID.", nameof(customerId));

            if (string.IsNullOrWhiteSpace(customerName))
                throw new ArgumentException("Customer name cannot be null or empty.", nameof(customerName));

            if (branchId == Guid.Empty)
                throw new ArgumentException("Branch ID must be a valid GUID.", nameof(branchId));

            if (string.IsNullOrWhiteSpace(branchName))
                throw new ArgumentException("Branch name cannot be null or empty.", nameof(branchName));

            Id = Guid.NewGuid();
            SaleNumber = saleNumber;
            SaleDate = DateTime.UtcNow;
            IsCancelled = false;

            CustomerId = customerId;
            CustomerName = customerName;
            BranchId = branchId;
            BranchName = branchName;
        }

        /// <summary>
        /// Adds an item to the sale.
        /// </summary>
        /// <param name="productId">The product ID of the item.</param>
        /// <param name="quantity">The quantity of the product.</param>
        /// <param name="unitPrice">The unit price of the product.</param>
        public void AddItem(int productId, int quantity, decimal unitPrice)
        {
            if (productId <= 0)
                throw new ArgumentException("ProductId must be greater than zero.", nameof(productId));

            if (quantity <= 0)
                throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");

            if (unitPrice <= 0)
                throw new ArgumentOutOfRangeException(nameof(unitPrice), "UnitPrice must be greater than zero.");

            var discount = CalculateDiscount(quantity);
            var item = new SaleItem(productId, quantity, unitPrice, discount);
            Items.Add(item);
        }

        /// <summary>
        /// Cancels the sale and marks all items as cancelled.
        /// </summary>
        public void CancelSale()
        {
            IsCancelled = true;
            foreach (var item in Items)
            {
                item.CancelItem();
            }
        }

        public void UpdateSaleNumber(string saleNumber)
        {
            if (string.IsNullOrWhiteSpace(saleNumber))
                throw new ArgumentException("Sale number cannot be null or empty.", nameof(saleNumber));

            SaleNumber = saleNumber;
        }

        public void UpdateSaleDate(DateTime saleDate)
        {
            if (saleDate == default)
                throw new ArgumentException("Sale date must be a valid value.", nameof(saleDate));

            SaleDate = saleDate;
        }

        public void UpdateCustomer(Guid customerId, string customerName)
        {
            if (customerId == Guid.Empty)
                throw new ArgumentException("Customer ID must be a valid GUID.", nameof(customerId));

            if (string.IsNullOrWhiteSpace(customerName))
                throw new ArgumentException("Customer name cannot be null or empty.", nameof(customerName));

            CustomerId = customerId;
            CustomerName = customerName;
        }

        public void UpdateBranch(Guid branchId, string branchName)
        {
            if (branchId == Guid.Empty)
                throw new ArgumentException("Branch ID must be a valid GUID.", nameof(branchId));

            if (string.IsNullOrWhiteSpace(branchName))
                throw new ArgumentException("Branch name cannot be null or empty.", nameof(branchName));

            BranchId = branchId;
            BranchName = branchName;
        }

        /// <summary>
        /// Creates a new sale from a shopping cart.
        /// </summary>
        /// <param name="cart">The shopping cart to convert into a sale.</param>
        /// <returns>A new instance of <see cref="Sale"/>.</returns>
        public static Sale FromCart(Cart.Cart? cart)
        {
            if (cart == null)
                throw new ArgumentNullException(nameof(cart));

            var sale = new Sale(
                Guid.NewGuid().ToString(),
                Guid.NewGuid(), // Replace with real customer ID
                $"Customer-{cart.UserId}", // Replace with real customer name
                Guid.NewGuid(), // Replace with real branch ID
                "Default Branch" // Replace with real branch name
            );

            foreach (var product in cart.Products)
            {
                sale.AddItem(product.ProductId, product.Quantity, product.UnitPrice);
            }

            return sale;
        }

        /// <summary>
        /// Calculates the discount based on the quantity.
        /// </summary>
        /// <param name="quantity">The quantity of the product.</param>
        /// <returns>The discount percentage.</returns>
        private decimal CalculateDiscount(int quantity)
        {
            if (quantity >= 10 && quantity <= 20) return 0.20m;
            if (quantity >= 4) return 0.10m;
            return 0.00m;
        }
    }
}
