namespace Gdn.Domain.Models.Settings;

/// <summary>
/// Authentication settings editable by administrators. The property defaults are what the
/// application uses until the section is saved for the first time, and a property added in a later
/// version materialises with its own default on sections saved before it existed.
/// </summary>
public class AuthenticationSettings : ISettingsSection
{
    /// <inheritdoc />
    public static string Key => "Authentication";

    /// <summary>
    /// Lifetime of the access token. Deliberately short: the token lives only in browser memory and
    /// cannot be revoked, so it bounds how long a deactivated user keeps working.
    /// </summary>
    public int AccessTokenMinutes { get; set; } = 15;

    /// <summary>
    /// Idle timeout of the session. Once the application is closed for longer than this, the refresh
    /// token has lapsed and a new sign in is required.
    /// </summary>
    public int SessionIdleTimeoutMinutes { get; set; } = 480;

    /// <summary>Failed sign in attempts tolerated before the account is locked.</summary>
    public int MaxFailedAccessAttempts { get; set; } = 5;

    /// <summary>How long a locked account stays locked.</summary>
    public int LockoutMinutes { get; set; } = 15;
}
