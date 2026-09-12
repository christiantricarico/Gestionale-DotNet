using FluentValidation;
using Gdn.Domain.Models.Settings;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.Settings;

public class UpdateAuthenticationSettings
{
    public record UpdateAuthenticationSettingsRequest(
        int AccessTokenMinutes,
        int SessionIdleTimeoutMinutes,
        int MaxFailedAccessAttempts,
        int LockoutMinutes);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("api/settings/authentication", HandlerAsync).WithTags(Tags.Settings).RequireAuthorization(Policies.AdminOnly);
        }
    }

    public sealed class Validator : AbstractValidator<UpdateAuthenticationSettingsRequest>
    {
        public Validator()
        {
            // The upper bounds matter as much as the lower ones: the access token cannot be revoked,
            // so a very long lifetime would widen the window in which a disabled account still works.
            RuleFor(e => e.AccessTokenMinutes).InclusiveBetween(1, 120);
            RuleFor(e => e.SessionIdleTimeoutMinutes).InclusiveBetween(1, 43200);
            RuleFor(e => e.MaxFailedAccessAttempts).InclusiveBetween(1, 50);
            RuleFor(e => e.LockoutMinutes).InclusiveBetween(1, 1440);
        }
    }

    private static async Task<IResult> HandlerAsync(
        UpdateAuthenticationSettingsRequest request,
        IValidator<UpdateAuthenticationSettingsRequest> validator,
        ISettingsService settingsService)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return ResultHelper.BadRequest(validationResult.Errors);

        await settingsService.SaveAsync(new AuthenticationSettings
        {
            AccessTokenMinutes = request.AccessTokenMinutes,
            SessionIdleTimeoutMinutes = request.SessionIdleTimeoutMinutes,
            MaxFailedAccessAttempts = request.MaxFailedAccessAttempts,
            LockoutMinutes = request.LockoutMinutes
        });

        return ResultHelper.NoContent();
    }
}
