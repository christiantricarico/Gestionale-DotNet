using Gdn.Persistence;
using Gdn.Web.Api.Vs.Endpoints;
using Microsoft.EntityFrameworkCore;

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

    public static async Task<IResult> Handler(AppDbContext context)
    {
        var taxRates = await context.TaxRateNatures
            .Select(e => new Response(e.Id, e.Code, e.Name))
            .ToListAsync();

        return TypedResults.Ok(taxRates);
    }
}
