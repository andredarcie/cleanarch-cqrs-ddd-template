using DevEval.Common.Helpers.Pagination;
using DevEval.Common.Helpers.Sorting;
using DevEval.Domain.Entities.User;
using DevEval.Domain.Enums;
using DevEval.Domain.Repositories;
using DevEval.ORM.Contexts;
using DevEval.ORM.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DevEval.ORM.Repositories
{
    /// <summary>
    /// Implementation of IUserRepository using Entity Framework Core.
    /// </summary>
    public class UserRepository : Repository<User, int>, IUserRepository
    {
        public UserRepository(DefaultContext context) : base(context)
        {
        }

        public async Task<PaginatedResult<User>> GetFilteredAsync(
            string? username,
            string? email,
            UserStatus? status,
            UserRole? role,
            PaginationParameters parameters)
        {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(username))
            {
                query = ApplyStringFilter(query, user => user.Username, username);
            }

            if (!string.IsNullOrWhiteSpace(email))
            {
                query = ApplyStringFilter(query, user => user.Email, email);
            }

            if (status.HasValue)
            {
                query = query.Where(user => user.Status == status.Value);
            }

            if (role.HasValue)
            {
                query = query.Where(user => user.Role == role.Value);
            }

            query = SortingHelper.ApplySorting(query, parameters.OrderBy);

            return await PaginationHelper.PaginateAsync(query, parameters.Page, parameters.PageSize);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        private static IQueryable<User> ApplyStringFilter(
            IQueryable<User> query,
            Expression<Func<User, string>> selector,
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

        private static Expression<Func<User, bool>> BuildEqualityExpression(
            Expression<Func<User, string>> selector,
            string value)
        {
            var parameter = selector.Parameters[0];
            var body = Expression.Equal(selector.Body, Expression.Constant(value));

            return Expression.Lambda<Func<User, bool>>(body, parameter);
        }

        private static Expression<Func<User, bool>> BuildLikeExpression(
            Expression<Func<User, string>> selector,
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

            return Expression.Lambda<Func<User, bool>>(body, parameter);
        }
    }
}
