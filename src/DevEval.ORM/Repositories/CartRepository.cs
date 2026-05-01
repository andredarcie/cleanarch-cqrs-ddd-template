using DevEval.Common.Helpers.Pagination;
using DevEval.Common.Helpers.Sorting;
using DevEval.Domain.Entities.Cart;
using DevEval.Domain.Repositories;
using DevEval.ORM.Contexts;
using DevEval.ORM.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace DevEval.ORM.Repositories
{
    /// <summary>
    /// Provides repository operations for managing shopping carts in the database.
    /// </summary>
    public class CartRepository : Repository<Cart, int>, ICartRepository
    {
        public CartRepository(DefaultContext context) : base(context)
        {
        }

        public async Task<PaginatedResult<Cart>> GetAllWithProductsAsync(PaginationParameters parameters)
        {
            var query = _dbSet.Include(c => c.Products).AsQueryable();

            query = SortingHelper.ApplySorting(query, parameters.OrderBy);

            return await PaginationHelper.PaginateAsync(query, parameters.Page, parameters.PageSize);
        }

        public async Task<PaginatedResult<Cart>> GetFilteredAsync(
            int? userId,
            DateTime? minDate,
            DateTime? maxDate,
            PaginationParameters parameters)
        {
            var query = _dbSet.Include(c => c.Products).AsQueryable();

            if (userId.HasValue)
            {
                query = query.Where(cart => cart.UserId == userId.Value);
            }

            if (minDate.HasValue)
            {
                query = query.Where(cart => cart.Date >= minDate.Value);
            }

            if (maxDate.HasValue)
            {
                query = query.Where(cart => cart.Date <= maxDate.Value);
            }

            query = SortingHelper.ApplySorting(query, parameters.OrderBy);

            return await PaginationHelper.PaginateAsync(query, parameters.Page, parameters.PageSize);
        }

        public async Task<Cart?> GetByIdWithProductsAsync(int id)
        {
            var entity = await _dbSet.Include(c => c.Products).FirstOrDefaultAsync(c => c.Id == id);

            if (entity == null)
            {
                throw new KeyNotFoundException($"The {typeof(Cart).Name} with ID {id} was not found.");
            }

            return entity;
        }
    }
}
