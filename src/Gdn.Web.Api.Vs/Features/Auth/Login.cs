using FluentValidation;
using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Gdn.Domain.Models.Settings;
using Gdn.Web.Api.Vs.Endpoints;
using Gdn.Web.Api.Vs.Features.Settings;
using Microsoft.AspNetCore.Identity;

namespace Gdn.Web.Api.Vs.Features.Auth;

public class Login
{
    public record LoginRequest(string Email, string Password);

    public record LoginResponse(
        string AccessToken,
        DateTime AccessTokenExpiresAtUtc,
        string RefreshToken,
        DateTime RefreshTokenExpiresAtUtc,
        string Email,
        string? FullName,
        string Role);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("api/auth/login", HandlerAsync)
               .WithTags(Tags.Auth)
               .AllowAnonymous()
               .RequireRateLimiting(Policies.AuthenticationRateLimit);
        }
    }

    public sealed class Validator : AbstractValidator<LoginRequest>
    {
        public Validator()
        {
            RuleFor(e => e.Email).NotEmpty().MaximumLength(255);
            RuleFor(e => e.Password).NotEmpty().MaximumLength(255);
        }
    }

    private static async Task<IResult> HandlerAsync(
        LoginRequest request,
        IValidator<LoginRequest> validator,
        IUserRepository userRepository,
        IPasswordHasher<User> passwordHasher,
        ISettingsService settingsService,
        TokenService tokenService,
        IUnitOfWork unitOfWork)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return ResultHelper.BadRequest(validationResult.Errors);

        var email = request.Email.Trim().ToLowerInvariant();
        var user = await userRepository.GetByEmailAsync(email);
        if (user is null)
            return ResultHelper.Unauthorized(AuthErrors.InvalidCredentials());

        var settings = await settingsService.GetAsync<AuthenticationSettings>();
        var now = DateTime.UtcNow;

        // A deactivated account reveals nothing: it answers exactly like a wrong password.
        if (!user.IsActive)
            return ResultHelper.Unauthorized(AuthErrors.InvalidCredentials());

        // Checked before the password, for two reasons. The caller is told why sign in fails instead
        // of being sent back to a password that cannot work anyway, and further attempts stop
        // counting, so a locked account is no longer pushed further into the future by every retry.
        if (user.IsLockedOut)
            return ResultHelper.Unauthorized(AuthErrors.AccountLocked(user.LockoutEndAt!.Value - now));

        var verification = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (verification is PasswordVerificationResult.Failed)
        {
            await RegisterFailedAttemptAsync(user, settings, now, userRepository, unitOfWork);
            return ResultHelper.Unauthorized(AuthErrors.InvalidCredentials());
        }

        user.FailedAccessAttempts = 0;
        user.LockoutEndAt = null;
        user.LastLoginAt = now;
        userRepository.Update(user);

        var accessTokenExpiresAt = now.AddMinutes(settings.AccessTokenMinutes);
        var accessToken = tokenService.CreateAccessToken(user, accessTokenExpiresAt);

        var refreshToken = TokenService.CreateRefreshToken();
        var refreshTokenExpiresAt = now.AddMinutes(settings.SessionIdleTimeoutMinutes);

        unitOfWork.GetRepository<IRefreshTokenRepository>().Add(new RefreshToken
        {
            UserId = user.Id,
            TokenHash = TokenService.HashRefreshToken(refreshToken),
            CreatedAt = now,
            ExpiresAt = refreshTokenExpiresAt
        });

        await unitOfWork.SaveChangesAsync();

        return ResultHelper.Ok(new LoginResponse(
            accessToken,
            accessTokenExpiresAt,
            refreshToken,
            refreshTokenExpiresAt,
            user.Email,
            user.FullName,
            user.Role));
    }

    private static async Task RegisterFailedAttemptAsync(
        User user,
        AuthenticationSettings settings,
        DateTime now,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        user.FailedAccessAttempts++;

        if (user.FailedAccessAttempts >= settings.MaxFailedAccessAttempts)
        {
            user.LockoutEndAt = now.AddMinutes(settings.LockoutMinutes);
            user.FailedAccessAttempts = 0;
        }

        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync();
    }
}
