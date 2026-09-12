using FluentValidation;
using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models.Settings;
using Gdn.Web.Api.Vs.Endpoints;
using Gdn.Web.Api.Vs.Features.Settings;

namespace Gdn.Web.Api.Vs.Features.Auth;

public class Refresh
{
    public record RefreshRequest(string RefreshToken);

    public record RefreshResponse(
        string AccessToken,
        DateTime AccessTokenExpiresAtUtc,
        DateTime RefreshTokenExpiresAtUtc,
        string Email,
        string? FullName,
        string Role);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("api/auth/refresh", HandlerAsync)
               .WithTags(Tags.Auth)
               .AllowAnonymous()
               .RequireRateLimiting(Policies.AuthenticationRateLimit);
        }
    }

    public sealed class Validator : AbstractValidator<RefreshRequest>
    {
        public Validator()
        {
            RuleFor(e => e.RefreshToken).NotEmpty().MaximumLength(255);
        }
    }

    private static async Task<IResult> HandlerAsync(
        RefreshRequest request,
        IValidator<RefreshRequest> validator,
        IRefreshTokenRepository refreshTokenRepository,
        ISettingsService settingsService,
        TokenService tokenService,
        IUnitOfWork unitOfWork)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return ResultHelper.BadRequest(validationResult.Errors);

        var hash = TokenService.HashRefreshToken(request.RefreshToken);
        var stored = await refreshTokenRepository.GetByHashAsync(hash);

        if (stored is null || !stored.IsActive || !stored.User.IsActive)
            return ResultHelper.Unauthorized(AuthErrors.InvalidRefreshToken());

        var settings = await settingsService.GetAsync<AuthenticationSettings>();
        var now = DateTime.UtcNow;

        // Sliding expiry: this is what keeps a session alive while the application stays open, and
        // what lets it lapse once the application has been closed for longer than the idle timeout.
        stored.ExpiresAt = now.AddMinutes(settings.SessionIdleTimeoutMinutes);
        refreshTokenRepository.Update(stored);

        var accessTokenExpiresAt = now.AddMinutes(settings.AccessTokenMinutes);
        var accessToken = tokenService.CreateAccessToken(stored.User, accessTokenExpiresAt);

        await unitOfWork.SaveChangesAsync();

        return ResultHelper.Ok(new RefreshResponse(
            accessToken,
            accessTokenExpiresAt,
            stored.ExpiresAt,
            stored.User.Email,
            stored.User.FullName,
            stored.User.Role));
    }
}
