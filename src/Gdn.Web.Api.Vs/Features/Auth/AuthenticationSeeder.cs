using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Gdn.Domain.Models.Enums;
using Microsoft.AspNetCore.Identity;

namespace Gdn.Web.Api.Vs.Features.Auth;

/// <summary>
/// Creates the first administrator when the users table is still empty, so a fresh database is
/// usable. Credentials come from configuration, never from source: use user secrets locally and the
/// application settings of the hosting environment in production.
/// </summary>
public sealed class AuthenticationSeeder(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IPasswordHasher<User> passwordHasher,
    IUnitOfWork unitOfWork,
    IConfiguration configuration,
    ILogger<AuthenticationSeeder> logger)
{
    public async Task SeedAsync()
    {
        await RemoveStaleRefreshTokensAsync();

        if (await userRepository.CountAsync() > 0)
            return;

        var email = configuration["AuthSettings:InitialAdminEmail"];
        var password = configuration["AuthSettings:InitialAdminPassword"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            logger.LogCritical(
                "Nessun utente presente e amministratore iniziale non configurato. Impostare AuthSettings:InitialAdminEmail e AuthSettings:InitialAdminPassword, altrimenti non sara possibile accedere.");
            return;
        }

        var admin = new User
        {
            Email = email.Trim().ToLowerInvariant(),
            FullName = "Amministratore",
            Role = UserRole.Admin,
            IsActive = true
        };

        admin.PasswordHash = passwordHasher.HashPassword(admin, password);

        userRepository.Add(admin);
        await unitOfWork.SaveChangesAsync();

        logger.LogInformation("Amministratore iniziale creato con email {Email}.", admin.Email);
    }

    /// <summary>
    /// Housekeeping at startup: revoked and long expired tokens are of no use and would otherwise
    /// make the table grow forever.
    /// </summary>
    private async Task RemoveStaleRefreshTokensAsync()
    {
        var removed = await refreshTokenRepository.DeleteStaleAsync(DateTime.UtcNow);

        if (removed > 0)
            logger.LogInformation("Rimossi {Count} refresh token non piu validi.", removed);
    }
}
