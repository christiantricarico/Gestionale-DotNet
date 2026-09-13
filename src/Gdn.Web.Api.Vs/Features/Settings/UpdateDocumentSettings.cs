using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models.Settings;
using Gdn.Web.Api.Vs.Endpoints;
using Gdn.Web.Api.Vs.Features.TaxRates;

namespace Gdn.Web.Api.Vs.Features.Settings;

public class UpdateDocumentSettings
{
    public record UpdateDocumentSettingsRequest(int? DefaultTaxRateId);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("api/settings/documents", HandlerAsync).WithTags(Tags.Settings).RequireAuthorization(Policies.AdminOnly);
        }
    }

    private static async Task<IResult> HandlerAsync(
        UpdateDocumentSettingsRequest request,
        ITaxRateRepository taxRateRepository,
        ISettingsService settingsService)
    {
        if (request.DefaultTaxRateId.HasValue)
        {
            var taxRate = await taxRateRepository.GetAsync(request.DefaultTaxRateId.Value);
            if (taxRate is null)
                return ResultHelper.NotFound(TaxRateErrors.NotFound(request.DefaultTaxRateId.Value));
        }

        await settingsService.SaveAsync(new DocumentSettings
        {
            DefaultTaxRateId = request.DefaultTaxRateId
        });

        return ResultHelper.NoContent();
    }
}
