using System.Security.Claims;
using Gdn.Domain.Data.Repositories;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.Auth;

public class GetCurrentUser
{
    public record GetCurrentUserResponse(int Id, string Email, string? FullName, string Role);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/auth/me", HandlerAsync).WithTags(Tags.Auth);
        }
    }

    private static async Task<IResult> HandlerAsync(ClaimsPrincipal principal, IUserRepository userRepository)
    {
        // Read from the database rather than from the token, so an account deactivated after the
        // token was issued is reported as unauthorised instead of still looking healthy.
        if (!int.TryParse(principal.FindFirstValue(TokenService.SubjectClaimType), out var userId))
            return ResultHelper.Unauthorized(AuthErrors.InvalidCredentials());

        var user = await userRepository.GetAsync(userId);
        if (user is null || !user.IsActive)
            return ResultHelper.Unauthorized(AuthErrors.InvalidCredentials());

        return ResultHelper.Ok(new GetCurrentUserResponse(user.Id, user.Email, user.FullName, user.Role));
    }
}
