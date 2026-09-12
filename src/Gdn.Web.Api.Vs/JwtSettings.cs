namespace Gdn.Web.Api.Vs;

/// <summary>
/// Settings of the token signing. These live in configuration rather than in the database because
/// they are needed before any database access, while the application is still starting up.
/// </summary>
public class JwtSettings
{
    public string Issuer { get; set; } = "Gdn";

    public string Audience { get; set; } = "GdnClient";

    /// <summary>
    /// Symmetric signing key, at least 32 bytes. It must never be committed: use user secrets
    /// locally and the application settings of the hosting environment in production.
    /// </summary>
    public string SigningKey { get; set; } = default!;
}
