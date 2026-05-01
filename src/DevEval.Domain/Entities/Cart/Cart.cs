namespace DevEval.Domain.Entities.Cart
{
    /// <summary>
    /// Represents a shopping cart containing products and associated metadata.
    /// </summary>
    public class Cart
    {
        /// <summary>
        /// Gets the unique identifier of the cart.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Gets the user ID associated with the cart.
        /// </summary>
        public int UserId { get; private set; }

        /// <summary>
        /// Gets the date when the cart was created or updated.
        /// </summary>
        public DateTime Date { get; private set; }

        /// <summary>
        /// Gets the list of products in the cart.
        /// </summary>
        public List<CartProduct> Products { get; set; } = [];

        /// <summary>
        /// Initializes a new instance of the <see cref="Cart"/> class.
        /// </summary>
        /// <param name="userId">The user ID associated with the cart.</param>
        /// <param name="date">The date the cart was created or updated.</param>
        /// <param name="products">The list of products in the cart.</param>
        public Cart(int userId, DateTime date, List<CartProduct>? products)
        {
            if (userId <= 0)
                throw new ArgumentException("UserId must be greater than zero.", nameof(userId));

            if (date == default)
                throw new ArgumentException("Date must be a valid value.", nameof(date));

            UserId = userId;
            Date = date;
            Products = products ?? [];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Cart"/> class with default values.
        /// </summary>
        public Cart(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("UserId must be greater than zero.", nameof(userId));

            UserId = userId;
            Date = DateTime.UtcNow;
        }

        /// <summary>
        /// Adds a product to the cart.
        /// </summary>
        /// <param name="product">The product to add.</param>
        public void AddProduct(CartProduct? product)
        {
            if (product != null)
            {
                Products.Add(product);
            }
            else
            {
                throw new ArgumentNullException(nameof(product));
            }
        }

        public void UpdateUserId(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("UserId must be greater than zero.", nameof(userId));

            UserId = userId;
        }

        public void UpdateDate(DateTime date)
        {
            if (date == default)
                throw new ArgumentException("Date must be a valid value.", nameof(date));

            Date = date;
        }

        /// <summary>
        /// Removes a product from the cart.
        /// </summary>
        /// <param name="productId">The ID of the product to remove.</param>
        public void RemoveProduct(int productId)
        {
            var product = Products.FirstOrDefault(p => p.ProductId == productId);
            if (product == null)
                throw new InvalidOperationException("Product not found in the cart.");

            Products.Remove(product);
        }

        /// <summary>
        /// Clears all products from the cart.
        /// </summary>
        public void ClearProducts()
        {
            Products.Clear();
        }
    }
}
