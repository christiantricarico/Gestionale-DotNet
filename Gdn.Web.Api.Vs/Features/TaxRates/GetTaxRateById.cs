using Gdn.Domain.Data.Repositories;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.TaxRates;

public class GetTaxRateById
{
    public record Response(int Id, string Code, string? Name, string? Description, decimal Rate, int? TaxRateNatureId);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/taxrates/{id}", Handler).WithTags(Tags.TaxRates);
        }
    }

    private static async Task<IResult> Handler(ITaxRateRepository taxRateRepository, int id)
    {
        var data = await taxRateRepository.GetAsync(id);

        return data is not null
            ? ResultHelper.Ok(new Response(data.Id, data.Code, data.Name, data.Description, data.Rate, data.TaxRateNatureId))
            : ResultHelper.NotFound();
    }
}
