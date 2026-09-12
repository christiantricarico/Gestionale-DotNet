using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Gdn.Domain.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Gdn.Persistence.Repositories;

internal sealed class UserRepository : Repository<User, int>, IUserRepository
{
    public UserRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    /// <inheritdoc />
    public Task<User?> GetByEmailAsync(string email)
        => _dbContext.Users.SingleOrDefaultAsync(e => e.Email == email);

    /// <inheritdoc />
    public Task<int> CountActiveAdminsExcludingAsync(int userId)
        => _dbContext.Users.CountAsync(e => e.Id != userId && e.IsActive && e.Role == UserRole.Admin);
}
