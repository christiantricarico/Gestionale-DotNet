using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Gdn.Domain.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Gdn.Web.Api.Vs.Features.Auth;

/// <summary>
/// Issues access tokens and refresh tokens. Short claim names are used instead of the long URI based
/// ones so the token stays small; the reading side is configured to match.
/// </summary>
public sealed class TokenService(IOptions<JwtSettings> jwtSettings)
{
    public const string SubjectClaimType = "sub";
    public const string EmailClaimType = "email";
    public const string RoleClaimType = "role";
    public const string NameClaimType = "name";

    private readonly JwtSettings _settings = jwtSettings.Value;

    /// <summary>Creates a signed access token for the user, expiring at the given instant.</summary>
    public string CreateAccessToken(User user, DateTime expiresAtUtc)
    {
        var handler = new JsonWebTokenHandler();

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _settings.Issuer,
            Audience = _settings.Audience,
            Expires = expiresAtUtc,
            Subject = new ClaimsIdentity(
            [
                new Claim(SubjectClaimType, user.Id.ToString()),
                new Claim(EmailClaimType, user.Email),
                new Claim(RoleClaimType, user.Role),
                new Claim(NameClaimType, user.FullName ?? user.Email)
            ]),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SigningKey)),
                SecurityAlgorithms.HmacSha256)
        };

        return handler.CreateToken(descriptor);
    }

    /// <summary>Creates an opaque refresh token. It carries no claims and is only ever matched by hash.</summary>
    public static string CreateRefreshToken()
        => Convert.ToBase64String(RandomNumberGenerator.GetBytes(48));

    /// <summary>Hashes a refresh token for storage, so the database never holds a usable token.</summary>
    public static string HashRefreshToken(string token)
        => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
}
