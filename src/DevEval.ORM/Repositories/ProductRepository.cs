using DevEval.Common.Helpers.Pagination;
using DevEval.Common.Helpers.Sorting;
using DevEval.Domain.Entities.Product;
using DevEval.Domain.Repositories;
using DevEval.ORM.Contexts;
using DevEval.ORM.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DevEval.ORM.Repositories
{
    /// <summary>
    /// Implementation of IProductRepository using Entity Framework Core.
    /// </summary>
    public class ProductRepository : Repository<Product, int>, IProductRepository
    {
        public ProductRepository(DefaultContext context) : base(context)
        {
        }

        public async Task<PaginatedResult<Product>> GetFilteredAsync(
            string? title,
            string? category,
            decimal? price,
            decimal? minPrice,
            decimal? maxPrice,
            PaginationParameters parameters)
        {
            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(title))
            {
                query = ApplyStringFilter(query, product => product.Title, title);
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                query = ApplyStringFilter(query, product => product.Category, category);
            }

            if (price.HasValue)
            {
                query = query.Where(product => product.Price == price.Value);
            }

            if (minPrice.HasValue)
            {
                query = query.Where(product => product.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(product => product.Price <= maxPrice.Value);
            }

            query = SortingHelper.ApplySorting(query, parameters.OrderBy);

            return await PaginationHelper.PaginateAsync(query, parameters.Page, parameters.PageSize);
        }

        public async Task<IEnumerable<string>> GetCategoriesAsync()
        {
            return await _context.Products
                                 .Select(p => p.Category)
                                 .Distinct()
                                 .ToListAsync();
        }

        public async Task<PaginatedResult<Product>> GetProductsByCategoryAsync(string category, PaginationParameters parameters)
        {
            var query = _context.Products.Where(p => p.Category == category);

            query = SortingHelper.ApplySorting(query, parameters.OrderBy);

            return await PaginationHelper.PaginateAsync(query, parameters.Page, parameters.PageSize);
        }

        private static IQueryable<Product> ApplyStringFilter(
            IQueryable<Product> query,
            Expression<Func<Product, string>> selector,
            string rawValue)
        {
            var startsWithWildcard = rawValue.StartsWith('*');
            var endsWithWildcard = rawValue.EndsWith('*');
            var normalizedValue = rawValue.Trim('*');

            if (string.IsNullOrWhiteSpace(normalizedValue))
            {
                return query;
            }

            if (startsWithWildcard && endsWithWildcard)
            {
                return query.Where(BuildLikeExpression(selector, $"%{normalizedValue}%"));
            }

            if (startsWithWildcard)
            {
                return query.Where(BuildLikeExpression(selector, $"%{normalizedValue}"));
            }

            if (endsWithWildcard)
            {
                return query.Where(BuildLikeExpression(selector, $"{normalizedValue}%"));
            }

            return query.Where(BuildEqualityExpression(selector, normalizedValue));
        }

        private static Expression<Func<Product, bool>> BuildEqualityExpression(
            Expression<Func<Product, string>> selector,
            string value)
        {
            var parameter = selector.Parameters[0];
            var body = Expression.Equal(selector.Body, Expression.Constant(value));

            return Expression.Lambda<Func<Product, bool>>(body, parameter);
        }

        private static Expression<Func<Product, bool>> BuildLikeExpression(
            Expression<Func<Product, string>> selector,
            string pattern)
        {
            var parameter = selector.Parameters[0];
            var body = Expression.Call(
                typeof(DbFunctionsExtensions),
                nameof(DbFunctionsExtensions.Like),
                Type.EmptyTypes,
                Expression.Property(null, typeof(EF), nameof(EF.Functions)),
                selector.Body,
                Expression.Constant(pattern));

            return Expression.Lambda<Func<Product, bool>>(body, parameter);
        }
    }
}
