using Gdn.Domain.Models.Bases;
using Gdn.Domain.Models.Enums;

namespace Gdn.Domain.Models;

/// <summary>
/// A user who can sign in to the application. Passwords are never stored in clear text:
/// <see cref="PasswordHash"/> holds the value produced by the password hasher.
/// </summary>
public class User : TrackedEntity<int>
{
    public string Email { get; set; } = default!;

    public string PasswordHash { get; set; } = default!;

    public string? FullName { get; set; }

    /// <summary>One of the constants declared in <see cref="UserRole"/>.</summary>
    public string Role { get; set; } = UserRole.Coworker;

    /// <summary>
    /// When <see langword="false"/> the user still exists but can no longer sign in, so the history
    /// tied to the account stays intact instead of being deleted.
    /// </summary>
    public bool IsActive { get; set; } = true;

    public int FailedAccessAttempts { get; set; }

    /// <summary>
    /// End of the lockout that follows too many failed attempts, or <see langword="null"/> when the
    /// account has never been locked.
    /// </summary>
    public DateTime? LockoutEndAt { get; set; }

    public DateTime? LastLoginAt { get; set; }

    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];

    /// <summary>Returns <see langword="true"/> while a lockout is still in force.</summary>
    public bool IsLockedOut => LockoutEndAt is not null && LockoutEndAt > DateTime.UtcNow;
}
