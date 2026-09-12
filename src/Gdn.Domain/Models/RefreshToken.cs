using Gdn.Domain.Models.Bases;

namespace Gdn.Domain.Models;

/// <summary>
/// Lets a client obtain a new access token without asking for credentials again. Only the SHA-256
/// hash of the token is persisted, so reading the table does not hand an attacker live sessions.
/// </summary>
public class RefreshToken : BaseEntity<long>
{
    public int UserId { get; set; }

    public User User { get; set; } = default!;

    public string TokenHash { get; set; } = default!;

    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Sliding expiry: each successful refresh pushes it forward by the configured idle timeout.
    /// This is what keeps a session alive while tabs stay open and lets it lapse once the app closes.
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    /// <summary>Returns <see langword="true"/> when the token is neither revoked nor expired.</summary>
    public bool IsActive => RevokedAt is null && ExpiresAt > DateTime.UtcNow;
}
