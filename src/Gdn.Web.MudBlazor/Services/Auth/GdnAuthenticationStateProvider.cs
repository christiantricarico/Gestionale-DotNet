using System.Security.Claims;
using Gdn.Web.MudBlazor.Models.Auth;
using Microsoft.AspNetCore.Components.Authorization;

namespace Gdn.Web.MudBlazor.Services.Auth;

/// <summary>
/// Holds the session. The access token stays in memory only, while the refresh token is persisted by
/// <see cref="TokenStore"/>, which is what makes a session survive a closed tab but not an idle
/// timeout.
/// </summary>
public sealed class GdnAuthenticationStateProvider(AuthClient authClient, TokenStore tokenStore) : AuthenticationStateProvider
{
    /// <summary>
    /// How long before expiry the access token is renewed. Renewing ahead of time means a request
    /// practically never fails with 401 just because the token lapsed mid flight.
    /// </summary>
    private static readonly TimeSpan RenewBefore = TimeSpan.FromSeconds(60);

    private static readonly ClaimsPrincipal Anonymous = new(new ClaimsIdentity());

    // Serialises renewals: several components loading at once must not each start their own.
    private readonly SemaphoreSlim _gate = new(1, 1);

    private ClaimsPrincipal _principal = Anonymous;
    private string? _accessToken;
    private DateTime _accessTokenExpiresAtUtc;
    private bool _initialized;

    /// <inheritdoc />
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        await EnsureInitializedAsync();

        return new AuthenticationState(_principal);
    }

    /// <summary>
    /// Returns a token good for the next request, renewing it first when it is about to expire, or
    /// <see langword="null"/> when there is no usable session.
    /// </summary>
    public async Task<string?> GetAccessTokenAsync()
    {
        await EnsureInitializedAsync();

        await _gate.WaitAsync();
        try
        {
            if (_accessToken is not null && _accessTokenExpiresAtUtc - RenewBefore > DateTime.UtcNow)
            {
                return _accessToken;
            }

            await RenewAsync();

            return _accessToken;
        }
        finally
        {
            _gate.Release();
        }
    }

    /// <summary>Signs in. Returns <see langword="null"/> on success, or the message to show otherwise.</summary>
    public async Task<string?> LoginAsync(string email, string password)
    {
        var (result, errorMessage) = await authClient.LoginAsync(email, password);

        if (result is null)
        {
            return errorMessage ?? "Accesso non riuscito.";
        }

        await _gate.WaitAsync();
        try
        {
            _initialized = true;
            _accessToken = result.AccessToken;
            _accessTokenExpiresAtUtc = result.AccessTokenExpiresAtUtc;
            _principal = BuildPrincipal(result.Email, result.FullName, result.Role);

            await tokenStore.WriteAsync(result.RefreshToken, result.RefreshTokenExpiresAtUtc);
        }
        finally
        {
            _gate.Release();
        }

        NotifyStateChanged();

        return null;
    }

    /// <summary>Signs out locally and revokes the refresh token server side.</summary>
    public async Task LogoutAsync()
    {
        var stored = await tokenStore.ReadAsync();
        var accessToken = _accessToken;

        await _gate.WaitAsync();
        try
        {
            ResetSession();
            await tokenStore.ClearAsync();
        }
        finally
        {
            _gate.Release();
        }

        if (stored is not null && accessToken is not null)
        {
            await authClient.LogoutAsync(stored.Token, accessToken);
        }

        NotifyStateChanged();
    }

    /// <summary>
    /// Drops the session after the server rejected a token. Called by the message handler on 401,
    /// which at that point means the session was revoked rather than merely expired.
    /// </summary>
    public async Task InvalidateAsync()
    {
        var wasAuthenticated = _principal.Identity?.IsAuthenticated is true;

        await _gate.WaitAsync();
        try
        {
            ResetSession();
            await tokenStore.ClearAsync();
        }
        finally
        {
            _gate.Release();
        }

        if (wasAuthenticated)
        {
            NotifyStateChanged();
        }
    }

    private async Task EnsureInitializedAsync()
    {
        if (_initialized)
        {
            return;
        }

        await _gate.WaitAsync();
        try
        {
            if (_initialized)
            {
                return;
            }

            _initialized = true;

            await RenewAsync();
        }
        finally
        {
            _gate.Release();
        }
    }

    /// <summary>Renews from the stored refresh token. The caller must already hold the gate.</summary>
    private async Task RenewAsync()
    {
        var stored = await tokenStore.ReadAsync();

        if (stored is null || stored.ExpiresAtUtc <= DateTime.UtcNow)
        {
            ResetSession();
            await tokenStore.ClearAsync();
            return;
        }

        var result = await authClient.RefreshAsync(stored.Token);

        if (result is null)
        {
            ResetSession();
            await tokenStore.ClearAsync();
            return;
        }

        _accessToken = result.AccessToken;
        _accessTokenExpiresAtUtc = result.AccessTokenExpiresAtUtc;
        _principal = BuildPrincipal(result.Email, result.FullName, result.Role);

        // The server slid the expiry forward, so the stored copy has to follow.
        await tokenStore.WriteAsync(stored.Token, result.RefreshTokenExpiresAtUtc);
    }

    private void ResetSession()
    {
        _accessToken = null;
        _accessTokenExpiresAtUtc = default;
        _principal = Anonymous;
    }

    private void NotifyStateChanged()
        => NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_principal)));

    private static ClaimsPrincipal BuildPrincipal(string email, string? fullName, string role)
    {
        var identity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Name, fullName ?? email),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role)
        ],
        authenticationType: "gdn",
        nameType: ClaimTypes.Name,
        roleType: ClaimTypes.Role);

        return new ClaimsPrincipal(identity);
    }
}
