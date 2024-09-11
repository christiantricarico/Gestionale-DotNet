using Gdn.Persistence;
using Gdn.Web.Api.Vs.Endpoints;
using Microsoft.EntityFrameworkCore;

namespace Gdn.Web.Api.Vs.Features.TaxRates;

public static class GetTaxRates
{
    public record Response(int Id, string Code, string? Name, string? Description, decimal Rate, int? TaxRateNatureId);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("taxrates", Handler).WithTags(Tags.TaxRates);
        }
    }

    public static async Task<IResult> Handler(AppDbContext context)
    {
        var taxRates = await context.TaxRates
            .Select(e => new Response(e.Id, e.Code, e.Name, e.Description, e.Rate, e.TaxRateNatureId))
            .ToListAsync();

        return TypedResults.Ok(taxRates);
        //return Results.Ok(taxRates);
    }
}
