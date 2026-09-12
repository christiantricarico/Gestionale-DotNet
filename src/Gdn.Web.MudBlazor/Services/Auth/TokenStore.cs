using Gdn.Web.MudBlazor.Models.Auth;
using Microsoft.JSInterop;

namespace Gdn.Web.MudBlazor.Services.Auth;

/// <summary>
/// Keeps the refresh token in browser local storage, so a session survives closing and reopening the
/// application until its idle timeout has elapsed. The access token is never stored: it lives only
/// in memory for the lifetime of the page.
/// </summary>
/// <remarks>
/// Local storage is read through plain JavaScript interop, which is the convention already used in
/// this project for the theme preference.
/// </remarks>
public sealed class TokenStore(IJSRuntime js)
{
    private const string TokenKey = "gdn.refreshToken";
    private const string ExpiresAtKey = "gdn.refreshExpiresAt";

    public async Task<StoredRefreshToken?> ReadAsync()
    {
        try
        {
            var token = await js.InvokeAsync<string?>("localStorage.getItem", TokenKey);
            var expiresAt = await js.InvokeAsync<string?>("localStorage.getItem", ExpiresAtKey);

            if (string.IsNullOrWhiteSpace(token) || !DateTime.TryParse(expiresAt, null, System.Globalization.DateTimeStyles.RoundtripKind, out var parsed))
            {
                return null;
            }

            return new StoredRefreshToken(token, parsed.ToUniversalTime());
        }
        catch (JSException)
        {
            // Local storage can be unavailable, for instance when the browser blocks site data.
            return null;
        }
    }

    public async Task WriteAsync(string token, DateTime expiresAtUtc)
    {
        try
        {
            await js.InvokeVoidAsync("localStorage.setItem", TokenKey, token);
            await js.InvokeVoidAsync("localStorage.setItem", ExpiresAtKey, expiresAtUtc.ToUniversalTime().ToString("O"));
        }
        catch (JSException)
        {
        }
    }

    public async Task ClearAsync()
    {
        try
        {
            await js.InvokeVoidAsync("localStorage.removeItem", TokenKey);
            await js.InvokeVoidAsync("localStorage.removeItem", ExpiresAtKey);
        }
        catch (JSException)
        {
        }
    }
}
