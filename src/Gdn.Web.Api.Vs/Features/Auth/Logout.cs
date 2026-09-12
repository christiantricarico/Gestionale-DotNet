using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.Auth;

public class Logout
{
    public record LogoutRequest(string RefreshToken);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("api/auth/logout", HandlerAsync).WithTags(Tags.Auth);
        }
    }

    private static async Task<IResult> HandlerAsync(
        LogoutRequest request,
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            return ResultHelper.NoContent();

        var hash = TokenService.HashRefreshToken(request.RefreshToken);
        var stored = await refreshTokenRepository.GetByHashAsync(hash);

        // Signing out never reports a failure: an unknown or already revoked token simply means the
        // session is already gone, which is exactly what the caller asked for.
        if (stored is not null && stored.RevokedAt is null)
        {
            stored.RevokedAt = DateTime.UtcNow;
            refreshTokenRepository.Update(stored);
            await unitOfWork.SaveChangesAsync();
        }

        return ResultHelper.NoContent();
    }
}
