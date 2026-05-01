using DevEval.Common;
using DevEval.Common.Helpers.Pagination;
using DevEval.Domain.Entities.User;
using DevEval.Domain.Enums;

namespace DevEval.Domain.Repositories
{
    /// <summary>
    /// Defines the operations for managing users in the repository.
    /// </summary>
    public interface IUserRepository : IRepository<User, int>
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<PaginatedResult<User>> GetFilteredAsync(
            string? username,
            string? email,
            UserStatus? status,
            UserRole? role,
            PaginationParameters parameters);
    }
}
