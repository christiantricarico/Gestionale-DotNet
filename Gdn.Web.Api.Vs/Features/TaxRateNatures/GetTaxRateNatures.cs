using Gdn.Domain.Data.Repositories;
using Gdn.Web.Api.Vs.Endpoints;

namespace Gdn.Web.Api.Vs.Features.TaxRateNatures;

public static class GetTaxRateNatures
{
    public record Response(int Id, string Code, string? Name);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/taxratenatures", Handler).WithTags(Tags.TaxRateNatures);
        }
    }

    public static async Task<IResult> Handler(ITaxRateNatureRepository taxRateNatureRepository)
    {
        var data = await taxRateNatureRepository.GetAllAsync();
        var responseData = data.Select(e => new Response(e.Id, e.Code, e.Name));

        return TypedResults.Ok(responseData);
    }
}
