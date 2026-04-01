using MudBlazor;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Gdn.Web.MudBlazor.Extensions;

/// <summary>
/// Extension methods for <see cref="ISnackbar"/> to display HTTP error responses.
/// </summary>
public static class SnackbarExtensions
{
    /// <summary>
    /// Shows an error snackbar if the HTTP response indicates failure.
    /// Extracts the error message from a RFC 7807 problem details response if available.
    /// </summary>
    /// <returns><see langword="true"/> if the response was successful; otherwise <see langword="false"/>.</returns>
    public static async Task<bool> ShowResponseErrorAsync(this ISnackbar snackbar, HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return true;
        }

        string message;

        try
        {
            var problem = await response.Content.ReadFromJsonAsync<ApiProblemDetails>();
            message = problem?.Detail ?? problem?.Title ?? "Si è verificato un errore non previsto.";
        }
        catch
        {
            message = "Si è verificato un errore non previsto.";
        }

        snackbar.Add(message, Severity.Error);

        return false;
    }

    private sealed record ApiProblemDetails(
        [property: JsonPropertyName("title")] string? Title,
        [property: JsonPropertyName("detail")] string? Detail);
}
