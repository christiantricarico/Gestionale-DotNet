using Gdn.Domain.Models;

namespace Gdn.Domain.Data.Repositories;

/// <summary>Refresh tokens repository. Tokens are always addressed by their hash, never in clear text.</summary>
public interface IRefreshTokenRepository : IRepository<RefreshToken, long>
{
    /// <summary>Finds a token by its SHA-256 hash, including the owning user.</summary>
    Task<RefreshToken?> GetByHashAsync(string tokenHash);

    /// <summary>Revokes every token still active for a user, ending all of their sessions.</summary>
    Task RevokeAllForUserAsync(int userId);

    /// <summary>Deletes expired and revoked tokens, so the table does not grow without bound.</summary>
    Task<int> DeleteStaleAsync(DateTime olderThanUtc);
}
