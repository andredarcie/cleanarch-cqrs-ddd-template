using Microsoft.EntityFrameworkCore;

namespace DevEval.Common.Helpers.Pagination
{
    public static class PaginationHelper
    {
        public static async Task<PaginatedResult<T>> PaginateAsync<T>(
            IQueryable<T> query,
            int page,
            int pageSize)
        {
            var totalItems = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<T>
            {
                Data = items,
                TotalItems = totalItems,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
            };
        }
    }
}
