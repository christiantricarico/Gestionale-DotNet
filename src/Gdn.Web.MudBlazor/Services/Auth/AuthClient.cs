using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Gdn.Web.MudBlazor.Models.Auth;

namespace Gdn.Web.MudBlazor.Services.Auth;

/// <summary>
/// Talks to the authentication endpoints over a bare <see cref="HttpClient"/>, deliberately outside
/// the handler pipeline used by the rest of the application.
/// </summary>
/// <remarks>
/// That separation is what breaks the circle: the pipeline handler asks the state provider for a
/// token, and the state provider calls here to renew it. Routing the renewal back through the
/// handler would make it call itself.
/// </remarks>
public sealed class AuthClient(HttpClient httpClient)
{
    public async Task<(LoginResult? Result, string? ErrorMessage)> LoginAsync(string email, string password)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync("auth/login", new { Email = email, Password = password });

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResult>();

                return result is not null
                    ? (result, null)
                    : (null, "Risposta non valida ricevuta dal server.");
            }

            return (null, await ReadErrorAsync(response));
        }
        catch (HttpRequestException)
        {
            return (null, "Impossibile contattare il server. Verifica la connessione.");
        }
    }

    /// <summary>
    /// Renews the access token. Returns <see langword="null"/> for any failure, because from the
    /// caller's point of view every failure means the same thing: the session is over.
    /// </summary>
    public async Task<RefreshResult?> RefreshAsync(string refreshToken)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync("auth/refresh", new { RefreshToken = refreshToken });

            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<RefreshResult>()
                : null;
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }

    /// <summary>Revokes the refresh token server side. Failures are ignored: the local session is cleared regardless.</summary>
    public async Task LogoutAsync(string refreshToken, string accessToken)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, "auth/logout")
            {
                Content = JsonContent.Create(new { RefreshToken = refreshToken })
            };

            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            await httpClient.SendAsync(request);
        }
        catch (HttpRequestException)
        {
        }
    }

    private static async Task<string> ReadErrorAsync(HttpResponseMessage response)
    {
        if (response.StatusCode is System.Net.HttpStatusCode.TooManyRequests)
            return "Troppi tentativi di accesso. Attendi qualche minuto e riprova.";

        try
        {
            var problem = await response.Content.ReadFromJsonAsync<ProblemPayload>();
            return problem?.Detail ?? problem?.Title ?? "Si è verificato un errore non previsto.";
        }
        catch
        {
            return "Si è verificato un errore non previsto.";
        }
    }

    private sealed record ProblemPayload(
        [property: JsonPropertyName("title")] string? Title,
        [property: JsonPropertyName("detail")] string? Detail);
}
