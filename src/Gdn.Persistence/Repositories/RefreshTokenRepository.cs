using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Gdn.Persistence.Repositories;

internal sealed class RefreshTokenRepository : Repository<RefreshToken, long>, IRefreshTokenRepository
{
    public RefreshTokenRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    /// <inheritdoc />
    public Task<RefreshToken?> GetByHashAsync(string tokenHash)
        => _dbContext.RefreshTokens
                     .Include(e => e.User)
                     .SingleOrDefaultAsync(e => e.TokenHash == tokenHash);

    /// <inheritdoc />
    public async Task RevokeAllForUserAsync(int userId)
    {
        var now = DateTime.UtcNow;

        await _dbContext.RefreshTokens
                        .Where(e => e.UserId == userId && e.RevokedAt == null)
                        .ExecuteUpdateAsync(setters => setters.SetProperty(e => e.RevokedAt, now));
    }

    /// <inheritdoc />
    public Task<int> DeleteStaleAsync(DateTime olderThanUtc)
        => _dbContext.RefreshTokens
                     .Where(e => e.ExpiresAt < olderThanUtc || e.RevokedAt != null)
                     .ExecuteDeleteAsync();
}
