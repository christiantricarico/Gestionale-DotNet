using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.TaxRates;

public class GetTaxRates
{
    public record Response(int Id, string Code, string? Name, string? Description, decimal Rate,
        int? TaxRateNatureId, string? TaxRateNatureName);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/taxrates", Handler).WithTags(Tags.TaxRates);
        }
    }

    private static async Task<IResult> Handler(ITaxRateRepository taxRateRepository)
    {
        var includes = new string[] { "TaxRateNature" };
        var data = await taxRateRepository.GetAllAsync(includes);
        var responseData = data.Select(e => MapResponse(e));

        return ResultHelper.Ok(responseData);
    }

    private static Response MapResponse(TaxRate entity)
    {
        return new(entity.Id, entity.Code, entity.Name, entity.Description, entity.Rate / 100,
            entity.TaxRateNatureId, entity.TaxRateNature?.Name);
    }
}
