using DevEval.Common;
using DevEval.Common.Helpers.Pagination;
using DevEval.Domain.Entities.Cart;

namespace DevEval.Domain.Repositories
{
    /// <summary>
    /// Defines the operations for managing shopping carts in the repository.
    /// </summary>
    public interface ICartRepository : IRepository<Cart, int>
    {
        Task<PaginatedResult<Cart>> GetAllWithProductsAsync(PaginationParameters parameters);
        Task<PaginatedResult<Cart>> GetFilteredAsync(
            int? userId,
            DateTime? minDate,
            DateTime? maxDate,
            PaginationParameters parameters);
        Task<Cart?> GetByIdWithProductsAsync(int id);
    }
}
