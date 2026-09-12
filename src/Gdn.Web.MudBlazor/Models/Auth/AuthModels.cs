using System.ComponentModel.DataAnnotations;

namespace Gdn.Web.MudBlazor.Models.Auth;

public class LoginModel
{
    [Required(ErrorMessage = "Email richiesta.")]
    [EmailAddress(ErrorMessage = "Email non valida.")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password richiesta.")]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;
}

/// <summary>Payload returned by a successful sign in.</summary>
public record LoginResult(
    string AccessToken,
    DateTime AccessTokenExpiresAtUtc,
    string RefreshToken,
    DateTime RefreshTokenExpiresAtUtc,
    string Email,
    string? FullName,
    string Role);

/// <summary>
/// Payload returned by a successful refresh. It carries no new refresh token, because the existing
/// one is reused and only its expiry moves forward.
/// </summary>
public record RefreshResult(
    string AccessToken,
    DateTime AccessTokenExpiresAtUtc,
    DateTime RefreshTokenExpiresAtUtc,
    string Email,
    string? FullName,
    string Role);

/// <summary>The refresh token as held in browser storage, together with the instant it lapses.</summary>
public record StoredRefreshToken(string Token, DateTime ExpiresAtUtc);
