using Gdn.Domain.Models.Settings;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.Settings;

public class GetDocumentSettings
{
    public record GetDocumentSettingsResponse(int? DefaultTaxRateId);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/settings/documents", HandlerAsync).WithTags(Tags.Settings).RequireAuthorization(Policies.AdminOnly);
        }
    }

    private static async Task<IResult> HandlerAsync(ISettingsService settingsService)
    {
        var settings = await settingsService.GetAsync<DocumentSettings>();

        return ResultHelper.Ok(new GetDocumentSettingsResponse(settings.DefaultTaxRateId));
    }
}
