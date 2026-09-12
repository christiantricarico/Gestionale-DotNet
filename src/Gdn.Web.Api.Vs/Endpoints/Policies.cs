namespace Gdn.Web.Api.Vs.Endpoints;

/// <summary>
/// Authorization policy and rate limit names. Every endpoint already requires an authenticated
/// caller through the fallback policy, so these mark the stricter requirements only.
/// </summary>
public static class Policies
{
    public const string AdminOnly = "AdminOnly";

    /// <summary>Rate limit applied to the endpoints that can be called without a token.</summary>
    public const string AuthenticationRateLimit = "AuthenticationRateLimit";
}
