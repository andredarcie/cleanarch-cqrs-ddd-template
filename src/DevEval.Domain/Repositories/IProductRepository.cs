using DevEval.Common;
using DevEval.Common.Helpers.Pagination;
using DevEval.Domain.Entities.Product;

namespace DevEval.Domain.Repositories
{
    /// <summary>
    /// Repository interface for managing product entities.
    /// </summary>
    public interface IProductRepository :  IRepository<Product, int>
    {
        /// <summary>
        /// Retrieves all unique product categories.
        /// </summary>
        /// <returns>A list of product categories.</returns>
        Task<IEnumerable<string>> GetCategoriesAsync();

        /// <summary>
        /// Retrieves a paginated list of products in a specific category with optional sorting.
        /// </summary>
        /// <param name="category">The name of the category.</param>
        /// <param name="parameters">Pagination and sorting parameters.</param>
        /// <returns>A paginated result of products in the specified category.</returns>
        Task<PaginatedResult<Product>> GetProductsByCategoryAsync(string category, PaginationParameters parameters);

        /// <summary>
        /// Retrieves products using the supported API filters.
        /// </summary>
        Task<PaginatedResult<Product>> GetFilteredAsync(
            string? title,
            string? category,
            decimal? price,
            decimal? minPrice,
            decimal? maxPrice,
            PaginationParameters parameters);
    }
}
