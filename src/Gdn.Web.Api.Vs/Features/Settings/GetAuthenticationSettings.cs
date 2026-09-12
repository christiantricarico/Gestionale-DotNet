using Gdn.Domain.Models.Settings;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.Settings;

public class GetAuthenticationSettings
{
    public record GetAuthenticationSettingsResponse(
        int AccessTokenMinutes,
        int SessionIdleTimeoutMinutes,
        int MaxFailedAccessAttempts,
        int LockoutMinutes);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/settings/authentication", HandlerAsync).WithTags(Tags.Settings).RequireAuthorization(Policies.AdminOnly);
        }
    }

    private static async Task<IResult> HandlerAsync(ISettingsService settingsService)
    {
        var settings = await settingsService.GetAsync<AuthenticationSettings>();

        return ResultHelper.Ok(new GetAuthenticationSettingsResponse(
            settings.AccessTokenMinutes,
            settings.SessionIdleTimeoutMinutes,
            settings.MaxFailedAccessAttempts,
            settings.LockoutMinutes));
    }
}
