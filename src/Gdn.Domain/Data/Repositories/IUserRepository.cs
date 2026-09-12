using Gdn.Domain.Models;

namespace Gdn.Domain.Data.Repositories;

/// <summary>
/// Users repository. The lookup and counting methods are declared here on purpose: the generic
/// <see cref="IRepository{TEntity, TId}"/> predicate overload materialises the whole table and
/// filters it in memory, which is not acceptable for a sign in path.
/// </summary>
public interface IUserRepository : IRepository<User, int>
{
    /// <summary>Finds a user by email, comparing case insensitively as the database collation does.</summary>
    Task<User?> GetByEmailAsync(string email);

    /// <summary>Counts the active administrators, excluding the given user.</summary>
    /// <remarks>
    /// Used to refuse any change that would leave the system without a usable administrator.
    /// </remarks>
    Task<int> CountActiveAdminsExcludingAsync(int userId);
}
