using System.Net;
using System.Net.Http.Headers;

namespace Gdn.Web.MudBlazor.Services.Auth;

/// <summary>
/// Attaches the bearer token to every API call and ends the session when the server rejects it.
/// </summary>
/// <remarks>
/// There is no retry on 401 by design. The token is renewed ahead of expiry, so a 401 does not mean
/// "expired, try again": it means the session was revoked, the account was disabled or the signing
/// key changed. Retrying would fail again, so the session is dropped and the user is sent to the
/// sign in page. A 403 is left untouched, because it is a legitimate answer to an allowed session.
/// </remarks>
public sealed class AuthorizationMessageHandler(GdnAuthenticationStateProvider authenticationStateProvider) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var accessToken = await authenticationStateProvider.GetAccessTokenAsync();

        if (accessToken is not null)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode is HttpStatusCode.Unauthorized)
        {
            await authenticationStateProvider.InvalidateAsync();
        }

        return response;
    }
}
